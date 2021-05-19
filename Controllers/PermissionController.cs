using Microsoft.AspNetCore.Mvc;
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
        [HttpPost("addPermission")]
        public async Task<ActionResult<string>> addPermission(PermissionDto dto)
        {
            try
            {
                Dictionary<string, string> claimDirectory = service.getClaims(Request.Headers["Autorisation"]);
                Guid id = Guid.Parse(claimDirectory.GetValueOrDefault("id"));

                if (!(await repository.hasPermission(id, "permission", "change"))) return BadRequest("No Permission to perform this action");

                await repository.addPermission(id, dto.permission);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion
    }
}
