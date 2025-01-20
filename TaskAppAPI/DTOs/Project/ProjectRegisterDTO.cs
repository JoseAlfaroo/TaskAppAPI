using System.ComponentModel.DataAnnotations;

namespace TaskAppAPI.DTOs.Project
{
    public class ProjectRegisterDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string HexColorCode { get; set; } = string.Empty;

        [Required]
        public bool IsFavorite { get; set; }

    }
}
