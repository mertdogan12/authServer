using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Collections.Generic;
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
        private readonly IPermissionRepository permissionRepository;
        private readonly IAuthContainerModel model;
        private readonly IAuthService service;

        public UserController(IUserRepository repository, IPermissionRepository permissionRepository)
        {
            this.repository = repository;
            this.permissionRepository = permissionRepository;
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
            try
            {
                Dictionary<string, string> claimsDictionary = service.getClaims(Request.Headers["Authorization"]);
                return Ok(JsonSerializer.Serialize(claimsDictionary).ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // users/changePassword
        [HttpPost("changePassword")]
        public async Task<ActionResult<string>> changePassword(ChancePasswordDto dto)
        {
            Guid id = Guid.Empty;

            try
            {
                Dictionary<string, string> claimsDictionary = service.getClaims(Request.Headers["Authorization"]);
                id = Guid.Parse(claimsDictionary.GetValueOrDefault("id"));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            string output = await repository.changePassword(dto.oldPassword, dto.newPassword, id);

            return (output == "Ok") ? Ok() : BadRequest(output);
        }

        // users/changeUsername
        [HttpPost("changeUsername")]
        public async Task<ActionResult<string>> changeUsername(ChanceUsernameDto dto)
        {
            Guid id = Guid.Empty;

            try
            {
                Dictionary<string, string> claimsDictionary = service.getClaims(Request.Headers["Authorization"]);
                id = Guid.Parse(claimsDictionary.GetValueOrDefault("id"));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            var output = await repository.changeUsername(id, dto.newUsername);

            return (output == "Ok") ? Ok() : BadRequest(output);
        }

        // users/getUsers
        [HttpGet("getUsers")]
        public async Task<ActionResult<User[]>> getUsers()
        {
            Guid id = Guid.Empty;

            try
            {
                Dictionary<string, string> claimsDictionary = service.getClaims(Request.Headers["Authorization"]);
                id = Guid.Parse(claimsDictionary.GetValueOrDefault("id"));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (!(await permissionRepository.hasPermission(id, "users", "getUsers"))) return BadRequest("You have not the Permission to perfrom this command");

            List<User> users = await repository.getUsers();
            List<UserDto> userDtos = new();
            users.ForEach((User user) =>
                    {
                        userDtos.Add(user.AsDto());
                    });

            return Ok(userDtos);
        }
        #endregion
    }
}
