using System;
using authServer.Dtos;

namespace authServer.Models
{
    public record User
    {
        public Guid id { get; init; }
        public string name { get; init; }
        public DateTimeOffset createDate { get; set; }
        public string hash { get; set; }

        public UserDto AsDto()
        {
            return new UserDto()
            {
                name = this.name,
                id = this.id,
                createDate = this.createDate
            };
        }
    }
}
