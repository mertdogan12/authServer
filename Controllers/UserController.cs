using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using authServer.Models;
using authServer.Dtos;
using System.Threading.Tasks;
using authServer.Repositories;
using authServer.Managers;

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

        // users/jwt
        [HttpGet("jwt")]
        public ActionResult<IEnumerable<Claim>> jwt()
        {
            if (string.IsNullOrWhiteSpace(Request.Headers["Authorization"]))
                return BadRequest("No Authorization token given");

            string token = Request.Headers["Authorization"].ToString().Split(' ')[1];

            IAuthContainerModel model = new JWTContainerModel();
            IAuthService service = new JWTService(model.secredKey);

            if (!service.isTokenValid(token))
                return BadRequest("Token is not valid");

            var claims = service.getTokenClaims(token).GetEnumerator();

            Dictionary<string, string> claimsDictionary = new();

            while (claims.MoveNext())
                claimsDictionary.Add(claims.Current.Type, claims.Current.Value);

            return Ok(JsonSerializer.Serialize(claimsDictionary).ToString());
        }
    }
}
