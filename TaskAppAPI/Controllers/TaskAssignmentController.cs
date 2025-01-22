using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Contexts;
using TaskAppAPI.DTOs;
using TaskAppAPI.DTOs.TaskAssignment;
using TaskAppAPI.Models;

namespace TaskAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskAssignmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskAssignmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> AssignTask([FromBody] TaskAssignmentRegisterDTO taskAssignmentRegisterDTO)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.TaskId == taskAssignmentRegisterDTO.TaskId);

            if (task == null)
            {
                return NotFound(new { message = "Tarea no encontrada" });
            }

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == task.ProjectId);

            if (project == null)
            {
                return NotFound(new { message = "No se encontró proyecto asociado a esta tarea" });
            }

            if (project.UserId != taskAssignmentRegisterDTO.CreadorProyectoId)
            {
                return BadRequest(new { message = "Solo el creador del proyecto puede asignar esta tarea" });
            }

            if (project.UserId == taskAssignmentRegisterDTO.AsignadoId)
            {
                return BadRequest(new { message = "El creador del proyecto no puede asignarse tareas a si mismo" });
            }

            // Busca si el posible asignado ya está invitado al proyecto
            var esInvitado = await _context.ProjectInvitations
                .AnyAsync(pi => pi.ProjectId == project.ProjectId && pi.UserId == taskAssignmentRegisterDTO.AsignadoId);

            // Si no lo está, no se puede asignar
            if (esInvitado == false)
            {
                return BadRequest(new { message = "Solo se puede asignar tareas a usuarios invitados al proyecto" });
            }

            // Si es que ya se asignó esta tarea a un usuario
            var taskAssignment = await _context.TaskAssignments.FirstOrDefaultAsync(ta => ta.TaskId == taskAssignmentRegisterDTO.TaskId && ta.UserId == taskAssignmentRegisterDTO.AsignadoId);

            if (taskAssignment != null)
            {
                return Conflict(new { message = $"Ya se asignó la tarea ({taskAssignmentRegisterDTO.TaskId}) al usuario ({taskAssignmentRegisterDTO.AsignadoId}) " });
            }

            try
            {
                var nuevaAsignacion = new TaskAssignmentModel
                {
                    TaskId = taskAssignmentRegisterDTO.TaskId,
                    UserId = taskAssignmentRegisterDTO.AsignadoId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.TaskAssignments.Add(nuevaAsignacion);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tarea asignada con éxito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("ListAssignedTasks/{userId}")]
        public async Task<IActionResult> GetAssignedTasksByUserId(int userId)
        {
            // Buscar las asignaciones de tareas de acuerdo a UserId
            var taskAssignments = await _context.TaskAssignments
                .Where(ta => ta.UserId == userId)
                .Include(ta => ta.Task)
                .ToListAsync();

            // Si el usuario no tiene asignaciones
            if (taskAssignments == null || !taskAssignments.Any())
            {
                return Ok(new { message = "No hay tareas asignadas a este usuario." });
            }

            // Lista objetos Task según asignaciones
            var tareas = taskAssignments.Select(ta => ta.Task).ToList();

            return Ok(tareas);
        }

    }
}
