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

        public PermissionController(IPermissionRepository repository)
        {
            this.repository = repository;
            model = new JWTContainerModel();
            service = new JWTService(model.secredKey);
        }

        #region Controller
        [HttpGet]
        public async Task<ActionResult<string[]>> getPermissions()
        {
            try
            {
                Dictionary<string, string> claimDirectory = service.getClaims(Request.Headers["Authorization"]);
                string name = claimDirectory.GetValueOrDefault(ClaimTypes.Name.ToString());

                if (!(await repository.hasPermission(name, "permission", "see"))) return BadRequest("No Permission to perform this action");

                return await repository.getPermissions(name);
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
                string name = claimDirectory.GetValueOrDefault(ClaimTypes.Name.ToString());

                if (!(await repository.hasPermission(name, "permission", "add"))) return BadRequest("No Permission to perform this action");

                await repository.addPermission(dto.username, dto.permission);

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
                string name = claimDirectory.GetValueOrDefault(ClaimTypes.Name.ToString());

                if (!(await repository.hasPermission(name, "permission", "remove"))) return BadRequest("No Permission to perform this action");

                await repository.removePermission(dto.username, dto.permission);

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
                string name = claimDirectory.GetValueOrDefault(ClaimTypes.Name.ToString());

                if (!(await repository.hasPermission(name, "permission", "see"))) return BadRequest("No Permission to perform this action");

                string[] permissions = dto.permission.Split('.');

                if (permissions.Length != 2) return BadRequest("Wrong permissionformat. Use permissiongroup.permission");

                return Ok(await repository.hasPermission(dto.username, permissions[0], permissions[1]));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion
    }
}
