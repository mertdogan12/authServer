using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using authServer.Dtos;
using System.Threading.Tasks;
using authServer.Repositories;
using authServer.Models;
using authServer.Managers;

namespace authServer.Controller
{
    [ApiController]
    [Route("permission")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository repository;
        private readonly IAuthContainerModel model;
        private readonly IAuthService service;

        public PermissionController(IPermissionRepository repository, IUserRepository userRepository)
        {
            this.repository = repository;
            model = new JWTContainerModel();
            service = new JWTService(model.secredKey);
        }

        #region Controller
        // permission/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<string[]>> getPermissions(Guid id)
        {
            try
            {
                Dictionary<string, string> claimDirectory = service.getClaims(Request.Headers["Authorization"]);
                Guid userid = Guid.Parse(claimDirectory.GetValueOrDefault("id"));

                if (id != userid && !(await repository.hasPermission(userid, "adminsettings", "see-permissions"))) return BadRequest("No Permission to perform this action");

                return await repository.getPermissions(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("addPermission")]
        public async Task<ActionResult<string>> addPermission(PermissionDto dto)
        {
            try
            {
                Dictionary<string, string> claimDirectory = service.getClaims(Request.Headers["Authorization"]);
                Guid id = Guid.Parse(claimDirectory.GetValueOrDefault("id"));

                if (!(await repository.hasPermission(id, "adminsettings", "add-permission"))) return BadRequest("No Permission to perform this action");

                await repository.addPermission(dto.id, dto.permission);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("removePermission")]
        public async Task<ActionResult<string>> removePermission(PermissionDto dto)
        {
            try
            {
                Dictionary<string, string> claimDirectory = service.getClaims(Request.Headers["Authorization"]);
                Guid id = Guid.Parse(claimDirectory.GetValueOrDefault("id"));

                if (!(await repository.hasPermission(id, "adminssettings", "remove-permission"))) return BadRequest("No Permission to perform this action");

                await repository.removePermission(dto.id, dto.permission);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("hasPermission")]
        public async Task<ActionResult<bool>> hasPermission(PermissionDto dto)
        {
            try
            {
                Dictionary<string, string> claimDirectory = service.getClaims(Request.Headers["Authorization"]);
                Guid id = Guid.Parse(claimDirectory.GetValueOrDefault("id"));

                if (!(await repository.hasPermission(id, "adminsettings", "see-permissions"))) return BadRequest("No Permission to perform this action");

                string[] permissions = dto.permission.Split('.');

                if (permissions.Length != 2) return BadRequest("Wrong permissionformat. Use permissiongroup.permission");

                return Ok(await repository.hasPermission(dto.id, permissions[0], permissions[1]));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion
    }
}
