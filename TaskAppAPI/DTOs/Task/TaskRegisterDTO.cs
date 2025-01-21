using System.ComponentModel.DataAnnotations;

namespace TaskAppAPI.DTOs.Task
{
    public class TaskRegisterDTO
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string TaskName { get; set; } = string.Empty;

        [Required]
        public string TaskDescription { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public byte Priority { get; set; }
    }
}
