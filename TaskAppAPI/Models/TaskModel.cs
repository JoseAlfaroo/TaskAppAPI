using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskAppAPI.Models
{
    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

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

        [Required]
        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }

        // Relacion con Project
        [JsonIgnore]
        [ForeignKey("ProjectId")]
        public ProjectModel? Project { get; set; }
    }
}
