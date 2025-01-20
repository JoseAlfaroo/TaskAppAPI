using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Contexts;
using TaskAppAPI.DTOs.User;
using TaskAppAPI.Models;

namespace TaskAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegisterDTO)
        {
            if (userRegisterDTO == null)
            {
                return BadRequest(new { Message = "Error en registro." });
            }

            var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == userRegisterDTO.Email);
            
            if (existingEmail != null)
            {
                return Conflict(new { Message = "Correo ya registrado." });
            }

            var existingUserName = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userRegisterDTO.UserName);

            if (existingUserName != null)
            {
                return Conflict(new { Message = "Nombre de usuario ya registrado." });
            }

            if(userRegisterDTO.Password == null || userRegisterDTO.Password.Length < 8 || userRegisterDTO.Password.Length > 12)
            {
                return BadRequest(new { Message = "La contraseña debe tener entre 8 y 12 caracteres" });
            }

            var role = await _context.Roles.FindAsync(userRegisterDTO.RoleId);

            if (role == null)
            {
                return BadRequest(new { Message = "El rol indicado no existe" });
            }
            
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDTO.Password);

            try
            {
                UserModel newUser = new UserModel
                {
                    UserId = 0,
                    FirstName = userRegisterDTO.FirstName,
                    LastName = userRegisterDTO.LastName,
                    UserName = userRegisterDTO.UserName,
                    Email = userRegisterDTO.Email,
                    Password = hashedPassword,
                    RoleId = userRegisterDTO.RoleId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Usuario registrado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            if (userLoginDTO == null || string.IsNullOrEmpty(userLoginDTO.Identifier) || string.IsNullOrEmpty(userLoginDTO.Password))
            {
                return BadRequest(new { Message = "Datos de inicio de sesión incompletos o incorrectos." });
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == userLoginDTO.Identifier || u.UserName == userLoginDTO.Identifier);

            if(user == null || !BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, user.Password))
            {
                return Unauthorized(new { Message = "Verifique sus credenciales e intente nuevamente." });
            }

            return Ok(new
            {
                Message = "Inicio de sesión exitoso",
                User = new
                {
                    user.UserId, user.FirstName, user.LastName, user.Email, user.UserName, UserRole = user?.Role?.RoleName
                }
            });

        }
    }
}
