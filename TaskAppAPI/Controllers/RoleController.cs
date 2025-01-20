using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
