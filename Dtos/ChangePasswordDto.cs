using System.ComponentModel.DataAnnotations;

namespace authServer.Dtos
{
    public record ChancePasswordDto
    {
        [Required]
        public string oldPassword { get; init; }
        [Required]
        public string newPassword { get; init; }
    }
}
