using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Contexts;
using TaskAppAPI.DTOs.Task;
using TaskAppAPI.Models;

namespace TaskAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskRegisterDTO taskRegisterDTO)
        {
            if (taskRegisterDTO == null)
            {
                return BadRequest(new { message = "Error al crear la tarea. Ingrese todos los campos" });
            }

            // Validando existencia del proyecto
            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == taskRegisterDTO.ProjectId);
            if (!projectExists)
            {
                return BadRequest(new { message = "El proyecto no existe." });
            }

            // Que el nombre de la tarea no sea nula o vacía
            if (string.IsNullOrEmpty(taskRegisterDTO.TaskName))
            {
                return BadRequest(new { message = "El nombre del proyecto no puede ser nulo o vacío." });
            }

            // Validando que no exista una tarea con el mismo nombre en el mismo proyecto
            var taskExists = await _context.Tasks
                .AnyAsync(t => t.TaskName == taskRegisterDTO.TaskName && t.ProjectId == taskRegisterDTO.ProjectId);

            if (taskExists)
            {
                return Conflict(new { message = "Ya existe una tarea con el mismo nombre en este proyecto." });
            }

            // Validación para que la descripción no sea nula
            if (string.IsNullOrEmpty(taskRegisterDTO.TaskDescription))
            {
                return BadRequest(new { message = "La descripción de la tarea no puede estar vacía." });
            }

            // Validando fecha obligatoria y en el futuro
            if (taskRegisterDTO?.DueDate == null || taskRegisterDTO.DueDate <= DateTime.UtcNow)
            {
                return BadRequest(new { message = "La fecha de vencimiento debe ser una fecha en el futuro." });
            }

            // Validando rango de prioridad (debe estar entre 1 y 4)
            if (taskRegisterDTO.Priority < 1 || taskRegisterDTO.Priority > 4)
            {
                return BadRequest(new { message = "La prioridad debe estar entre 1 y 4. La prioridad 1 es la más urgente." });
            }

            // Creación de la tarea
            var newTask = new TaskModel
            {
                ProjectId = taskRegisterDTO.ProjectId,
                TaskName = taskRegisterDTO.TaskName,
                TaskDescription = taskRegisterDTO.TaskDescription,
                DueDate = taskRegisterDTO.DueDate,
                Priority = taskRegisterDTO.Priority,
                IsCompleted = false, // Al crearse, no están completadas
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tarea creada exitosamente.", newTask });
        }

        [HttpGet("GetTasksByProject/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
            {
                return NotFound(new { Message = "El proyecto indicado no existe" });
            }

            var tasks = await _context.Tasks
                .Where(task => task.ProjectId == projectId)
                .ToListAsync();

            if (!tasks.Any())
            {
                return Ok(new { Message = "Este proyecto no tiene ninguna tarea" });
            }

            return Ok(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return NotFound(new { Message = "Esta tarea no existe" });
            }

            return Ok(task);
        }
    }
}
