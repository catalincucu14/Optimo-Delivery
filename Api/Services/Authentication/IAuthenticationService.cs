using Models.Resources;
using System.Security.Claims;

namespace Api.Services.Authentication
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Function used to generate the JWT 
        /// </summary>
        public string GenerateJwtToken(List<Claim> claims);

        /// <summary>
        /// Function used to get the claims for an admin
        /// </summary>
        public List<Claim> GetClaimsAdmin(Admin admin);

        /// <summary>
        /// Function used to get the claims for an driver
        /// </summary>
        public List<Claim> GetClaimsDriver(Driver driver);
    }
}
