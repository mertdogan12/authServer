using System;

namespace authServer.Models
{
    public record User
    {
        public Guid id { get; init; }
        public string name { get; init; }
        public DateTimeOffset createDate { get; set; }
        public string hash { get; set; }
    }
}
