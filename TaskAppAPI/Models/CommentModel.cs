using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskAppAPI.Models
{
    public class CommentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Relaciones con Task y User
        [JsonIgnore]
        [ForeignKey("TaskId")]
        public TaskModel? Task { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public UserModel? User { get; set; }
    }
}
