using Microsoft.AspNetCore.Mvc;
using System;
using authServer.Models;
using authServer.Dtos;
using System.Threading.Tasks;
using authServer.Repositories;

namespace authServer.Controller
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository repository;

        public UserController(IUserRepository repository)
        {
            this.repository = repository;
        }

        // users/register
        [HttpPost("register")]
        public async Task<ActionResult<RegisterDto>> register(RegisterDto dto)
        {
            if (!(await repository.getUser(dto.name) is null)) return BadRequest("User is already taken");

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

        // users/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> login(RegisterDto dto)
        {
            string jwsToken = await repository.login(dto.name, dto.password);

            if (jwsToken is null) return BadRequest("Password or Username is false");

            return Ok(jwsToken);
        }
    }
}
