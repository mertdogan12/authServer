using System;
using System.Security.Claims;
using System.Collections.Generic;
using authServer.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace authServer.Managers
{
    public class JWTService : IAuthService
    {
        #region Members
        /// <summary>
        /// The secred key we use to encrypt out token with.
        /// </summary>
        public string secredKey { get; set; }
        #endregion

        #region Constructor
        public JWTService(string secredKey)
        {
            this.secredKey = secredKey;
        }
        #endregion

        #region Private Methods
        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(secredKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private TokenValidationParameters getTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey()
            };
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Validates whether a given token is valid or not, and return true in cse the token is valid otherwise it will return false;
        /// </summary>
        /// <param name="token">The Token</param>
        /// <returns>If the token is valid</return>
        public bool isTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty");

            TokenValidationParameters tokenValidationParameters = getTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

            try
            {
                ClaimsPrincipal tokenvalid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Generates token by given model.
        /// Validates whether the given model is valid, then gets the symmentric key.
        /// Encrypt the token and return it.
        /// </summary>
        /// <param name="model">The given model</param>
        /// <returns>Generated token</returns>
        public string generateToken(IAuthContainerModel model)
        {
            if (model == null || model.claims == null || model.claims.Length == 0)
                throw new ArgumentException("Arguments to create token are not valid.");

            SecurityTokenDescriptor securityTokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(model.claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(model.expireMinutes)),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), model.securityAlgorithm)
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }

        /// <summary>
        /// Receives the claims of token by given token as string.
        /// </summary>
        /// <remarks>
        /// Pay attention, one ht etoken id Fake the method will thor an exception.
        /// </remarks>
        /// <param name="token"></param>
        /// <returns>Ienumerable of claims for the given token.</returns>
        public IEnumerable<Claim> getTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = getTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return tokenValid.Claims;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
