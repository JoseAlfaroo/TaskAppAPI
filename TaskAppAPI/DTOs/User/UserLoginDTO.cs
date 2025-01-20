using System.ComponentModel.DataAnnotations;

namespace TaskAppAPI.DTOs.User
{
    public class UserLoginDTO
    {
        [Required]
        public string Identifier{ get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
