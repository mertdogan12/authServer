using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
        private readonly IAuthContainerModel model;
        private readonly IAuthService service;

        public UserController(IUserRepository repository)
        {
            this.repository = repository;
            model = new JWTContainerModel();
            service = new JWTService(model.secredKey);
        }

        #region Controller
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
        public ActionResult<IEnumerable<ClaimDto>> jwt()
        {
            if (string.IsNullOrWhiteSpace(Request.Headers["Authorization"]))
                return BadRequest("No Authorization token given");

            string[] tokenArray = Request.Headers["Authorization"].ToString().Split(' ');

            if (tokenArray.Length != 2)
                return BadRequest("No Authorization token is given");

            string token = tokenArray[1];

            if (!service.isTokenValid(token))
                return BadRequest("Token is not valid");

            var claims = service.getTokenClaims(token).GetEnumerator();

            Dictionary<string, string> claimsDictionary = new();

            while (claims.MoveNext())
                claimsDictionary.Add(claims.Current.Type, claims.Current.Value);

            return Ok(JsonSerializer.Serialize(claimsDictionary).ToString());
        }

        // users/changePassword
        [HttpPost("changePassword")]
        public async Task<ActionResult<string>> changePassword(ChancePasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers["Authorization"]))
                return BadRequest("No Authorization token given");

            string[] tokenArray = Request.Headers["Authorization"].ToString().Split(' ');

            if (tokenArray.Length != 2)
                return BadRequest("No Authorization token is given");

            string token = tokenArray[1];

            if (!service.isTokenValid(token))
                return BadRequest("Token is not valid");

            var claims = service.getTokenClaims(token).GetEnumerator();
            string username = "";

            while (claims.MoveNext())
                if (claims.Current.Type.Equals(ClaimTypes.Name))
                    username = claims.Current.Value;

            string output = await repository.changePassword(dto.oldPassword, dto.newPassword, username);

            return (output == "Ok") ? Ok() : BadRequest(output);
        }

        // users/changeUsername
        [HttpPost("changeUsername")]
        public async Task<ActionResult<string>> changeUsername(ChanceUsernameDto dto)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers["Authorization"]))
                return BadRequest("No Authorization token given");

            string[] tokenArray = Request.Headers["Authorization"].ToString().Split(' ');

            if (tokenArray.Length != 2)
                return BadRequest("No Authorization token is given");

            string token = tokenArray[1];

            if (!service.isTokenValid(token))
                return BadRequest("Token is not valid");

            var claims = service.getTokenClaims(token).GetEnumerator();
            string username = "";

            while (claims.MoveNext())
                if (claims.Current.Type.Equals(ClaimTypes.Name))
                    username = claims.Current.Value;

            var output = await repository.changeUsername(username, dto.newUsername);

            return (output == "Ok") ? Ok() : BadRequest(output);
        }
        #endregion
    }
}
