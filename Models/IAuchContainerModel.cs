using System.Security.Claims;

namespace authServer.Models
{
    public interface IAuthContainerModel
    {
        #region Members
        string secredKey { get; set; }
        string securityAlgorithm { get; set; }
        int expireMinutes { get; set; }

        Claim[] claims { get; set; }
        #endregion
    }
}
