using System;

namespace authServer.Models
{
    public record Permission
    {
        public Guid id { get; init; }
        public string[] permissions { get; init; }
    }
}
