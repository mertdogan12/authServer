using System.ComponentModel.DataAnnotations;

namespace authServer.Dtos
{
    public record ChanceUsernameDto
    {
        [Required]
        public string newUsername { get; init; }
    }
}
