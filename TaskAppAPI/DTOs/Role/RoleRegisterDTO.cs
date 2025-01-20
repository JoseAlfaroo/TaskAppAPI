using System.ComponentModel.DataAnnotations;

namespace TaskAppAPI.DTOs.Role
{
    public class RoleRegisterDTO
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
