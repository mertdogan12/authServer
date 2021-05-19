using Microsoft.AspNetCore.Mvc;
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

        #endregion
    }
}
