using System.ComponentModel.DataAnnotations;

namespace TaskAppAPI.DTOs.TaskAssignment
{
    public class TaskAssignmentRegisterDTO
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public int CreadorProyectoId { get; set; }

        [Required]
        public int AsignadoId { get; set; }
    }
}
