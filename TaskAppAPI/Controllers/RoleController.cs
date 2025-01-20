using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Contexts;
using TaskAppAPI.DTOs.Role;
using TaskAppAPI.DTOs.User;
using TaskAppAPI.Models;

namespace TaskAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleRegisterDTO roleRegisterDTO)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.RoleName == roleRegisterDTO.RoleName);

            if (roleExists)
            {
                return Conflict(new { Message = "Este rol ya existe" });
            }

            try
            {
                RoleModel newRole = new RoleModel
                {
                    RoleId = 0,
                    RoleName = roleRegisterDTO.RoleName
                };

                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Rol registrado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _context.Roles.ToListAsync();

            return Ok(roles);
        }
    }
}
