using System;
using System.ComponentModel.DataAnnotations;

namespace authServer.Dtos
{
    public record PermissionDto
    {
        [Required]
        public string permission { get; init; }
        [Required]
        public Guid id { get; init; }
    }
}
