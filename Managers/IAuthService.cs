using System.Collections.Generic;
using System.Security.Claims;
using authServer.Models;

namespace authServer.Managers
{
    public interface IAuthService
    {
        string secredKey { get; set; }

        bool isTokenValid(string token);
        string generateToken(IAuthContainerModel model);
        IEnumerable<Claim> getTokenClaims(string token);
        Dictionary<string, string> getClaims(string authorizationHeader);
    }
}
