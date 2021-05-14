using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace authServer.Models
{
    public class JWTContainerModel : IAuthContainerModel
    {
        #region Members
        public string secredKey { get; set; } = Startup.secredKey;
        public string securityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public int expireMinutes { get; set; } = 10080; // 7d

        public Claim[] claims { get; set; }
        #endregion
    }
}
