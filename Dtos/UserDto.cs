using System.ComponentModel.DataAnnotations;
using System;

namespace authServer.Dtos
{
    public record UserDto
    {
        [Required]
        public Guid id { get; init; }
        [Required]
        public string name { get; init; }
        [Required]
        public DateTimeOffset createDate { get; init; }
    }
}
