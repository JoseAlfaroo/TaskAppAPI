using System.ComponentModel.DataAnnotations;

namespace TaskAppAPI.DTOs.ProjectInvitation
{
    public class ProjectInvitationRegisterDTO
    {
        [Required]
        public int UsuarioCreadorId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int UsuarioInvitadoId { get; set; }
    }
}
