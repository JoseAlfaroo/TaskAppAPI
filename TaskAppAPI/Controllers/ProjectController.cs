using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Contexts;
using TaskAppAPI.DTOs.Project;
using TaskAppAPI.Models;

namespace TaskAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectRegisterDTO projectRegisterDTO)
        {
            if (projectRegisterDTO == null)
            {
                return BadRequest(new { message = "Error en registro." });
            }

            // Validación de la existencia del usuario
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == projectRegisterDTO.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "El usuario no existe" });
            }

            // Validación para que el proyecto no tenga el mismo nombre para el mismo usuario
            var projectExists = await _context.Projects.AnyAsync(p => p.Name == projectRegisterDTO.Name && p.UserId == projectRegisterDTO.UserId);
            if (projectExists)
            {
                return BadRequest(new { message = "Ya existe un proyecto con el mismo nombre para este usuario" });
            }

            // Validación de que el nombre del proyecto no sea nulo o vacío
            if (string.IsNullOrEmpty(projectRegisterDTO.Name))
            {
                return BadRequest(new { message = "El nombre del proyecto no puede ser nulo o vacío." });
            }

            // Validación del formato HEX del color
            if (!string.IsNullOrEmpty(projectRegisterDTO.HexColorCode))
            {
                var hexColorRegex = @"^#[0-9A-Fa-f]{6}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(projectRegisterDTO.HexColorCode, hexColorRegex))
                {
                    return BadRequest(new { message = "El código de color debe ser un valor HEX válido (ej. #FFFFFF)." });
                }
            }

            try
            {
                // Creación del proyecto
                var newProject = new ProjectModel
                {                
                    UserId = projectRegisterDTO.UserId,
                    Name = projectRegisterDTO.Name,
                    HexColorCode = projectRegisterDTO.HexColorCode,
                    IsFavorite = projectRegisterDTO.IsFavorite,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Projects.Add(newProject);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Proyecto creado exitosamente",
                    UserFullName = $"{user.FirstName} {user.LastName}",
                    newProject
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _context.Projects
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound(new { message = "Proyecto no encontrado" });
            }

            return Ok(new
            {
                UserFullName = $"{project.User!.FirstName} {project.User!.LastName}",
                project
            });
        }

        [HttpGet("GetProjectsByUser/{UserId}")]
        public async Task<IActionResult> GetProjectsByUser(int UserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == UserId);
            if (user == null)
            {
                return BadRequest(new { message = "El usuario no existe" });
            }

            // Obtener proyectos creados por UserId
            var projects = await _context.Projects.Where(p => p.UserId == UserId).ToListAsync();

            if (!projects.Any())
            {
                return BadRequest(new { Message = "Este usuario no tiene proyectos" });
            }

            var userFullName = $"{user!.FirstName} {user!.LastName}";

            return Ok(new { userFullName, projects });
        }
    }
}
