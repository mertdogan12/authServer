using System.ComponentModel.DataAnnotations;

namespace authServer.Dtos
{
    public record ClaimDto
    {
        [Required]
        public string typeName { get; init; }
        [Required]
        public string value { get; init; }
    }
}
