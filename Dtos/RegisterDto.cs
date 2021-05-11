using System.ComponentModel.DataAnnotations;

namespace authServer.Dtos
{
    public record RegisterDto
    {
        [Required]
        public string name { get; init; }
        [Required]
        public string password { get; init; }
    }
}
