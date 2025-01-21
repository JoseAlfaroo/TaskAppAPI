using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Contexts;
using TaskAppAPI.DTOs.ProjectInvitation;
using TaskAppAPI.Models;

namespace TaskAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectInvitationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectInvitationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> InvitarUsuarios([FromBody] ProjectInvitationRegisterDTO projectInvitationRegisterDTO)
        {
            // Busca proyecto
            var project = await _context.Projects.FindAsync(projectInvitationRegisterDTO.ProjectId);

            // Si no existe proyecto
            if (project == null)
            {
                return NotFound(new { message = "No se encontró este proyecto" });
            }

            // Solo el creador debe poder invitar
            if (project.UserId != projectInvitationRegisterDTO.UsuarioCreadorId)
            {
                return BadRequest(new { message = "Solo el creador de proyecto puede invitar usuarios." });
            }

            // El creador del proyecto no puede invitarse a si mismo
            if (project.UserId == projectInvitationRegisterDTO.UsuarioInvitadoId)
            {
                return Conflict(new { message = "El creador del proyecto no puede ser invitado" });
            }

            // Contar los invitados actuales del proyecto
            var cantidadInvitados = await _context.ProjectInvitations
                .CountAsync(pi => pi.ProjectId == projectInvitationRegisterDTO.ProjectId);

            // Validar que no supere el límite de 10 invitados
            if (cantidadInvitados >= 10)
            {
                return BadRequest(new { message = "El proyecto ya tiene el número máximo permitido de invitados (10)." });
            }

            // Busca invitado según Id
            var invitado = await _context.Users.FindAsync(projectInvitationRegisterDTO.UsuarioInvitadoId);

            // Si no existe el invitado
            if (invitado == null)
            {
                return BadRequest(new { message = $"No se encontró al invitado con id {projectInvitationRegisterDTO.UsuarioInvitadoId}." });
            }

            // Buscar invitación
            var invitacion = await _context.ProjectInvitations.FirstOrDefaultAsync(pi => pi.ProjectId == projectInvitationRegisterDTO.ProjectId && pi.UserId == projectInvitationRegisterDTO.UsuarioInvitadoId);

            // Si la invitación existe
            if (invitacion != null)
            {
                return Conflict(new { message = $"Ya se invitó al usuario {projectInvitationRegisterDTO.UsuarioInvitadoId} al proyecto {projectInvitationRegisterDTO.ProjectId}" });
            }

            // Si no existe la invitación, continua normalmente
            var nuevaInvitacion = new ProjectInvitationModel
            {
                ProjectId = projectInvitationRegisterDTO.ProjectId,
                UserId = projectInvitationRegisterDTO.UsuarioInvitadoId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            try
            {
                _context.ProjectInvitations.Add(nuevaInvitacion);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Se invitó al usuario '{invitado.UserName}' (ID: {invitado.UserId}) a proyecto '{project.Name}' (ID: {project.ProjectId})" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("ListInvitedUsers")]
        public async Task<IActionResult> GetInvitedUsersByProjectId([FromQuery] int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if(project == null)
            {
                return NotFound(new { Message = "Proyecto no encontrado" });
            }

            var projectInvitations = await _context.ProjectInvitations
                .Include(pi => pi.User)
                .Where(pi => pi.ProjectId == projectId)
                .ToListAsync();

            var invitados = projectInvitations
                .Select(pi => new
                {
                    pi.User!.UserId,
                    UserFullName = $"{pi.User.FirstName} {pi.User.LastName}"
                })
                .ToList();

            if (!invitados.Any())
            {
                return Ok(new { Message = "Este proyecto no tiene ningún invitado" });
            }

            return Ok(invitados);
        }
    }
}
