using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using authServer.Dtos;
using authServer.Repositories;

namespace authServer.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository repository;

        public LoginController(IUserRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<string>> login(RegisterDto dto)
        {
            string jwsToken = await repository.login(dto.name, dto.password);

            if (jwsToken is null) return "Password or Username is false";

            return Ok(jwsToken);
        }
    }
}
