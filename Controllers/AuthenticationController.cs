using Microsoft.AspNetCore.Mvc;
using System;
using authServer.Models;
using authServer.Dtos;
using System.Threading.Tasks;
using authServer.Repositories;

namespace authServer.Controller
{
    [ApiController]
    [Route("register")]
    public class RegisterController : ControllerBase
    {
        private readonly IUserRepository repository;

        public RegisterController(IUserRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<RegisterDto>> register(RegisterDto dto)
        {
            if (!(await repository.getUser(dto.name) is null)) return NotFound();

            User user = new()
            {
                name = dto.name,
                createDate = DateTimeOffset.UtcNow,
                id = Guid.NewGuid(),
                hash = BCrypt.Net.BCrypt.HashPassword(dto.password)
            };

            await repository.register(user);

            return Ok();
        }
    }
}
