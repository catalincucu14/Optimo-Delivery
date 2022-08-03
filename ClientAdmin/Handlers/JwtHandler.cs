using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClientAdmin.Handlers
{
    public class JwtHandler
    {
        /// <summary>
        /// Function used to decode a JWT
        /// </summary>
        public static JwtSecurityToken Decode(string jwtToken) => (new JwtSecurityTokenHandler()).ReadJwtToken(jwtToken);

        /// <summary>
        /// Function used check if a JWT is expired (based on expiry date)
        /// </summary>
        public static bool IsExpired(JwtSecurityToken decodedJwtToken) => decodedJwtToken.ValidFrom > DateTime.UtcNow || decodedJwtToken.ValidTo < DateTime.UtcNow;

        /// <summary>
        /// Function used to check if a JWT has any claims
        /// </summary>
        public static bool HasNoClaims(JwtSecurityToken decodedJwtToken) => decodedJwtToken.Claims.ToList().Count == 0;

        /// <summary>
        /// Function used to return the claims if a JWT is not valid
        /// </summary>
        public static ClaimsPrincipal GetClaimsNotAuthorized() => new ClaimsPrincipal(new ClaimsIdentity());

        /// <summary>
        /// Function used to return the claims if a JWT is valid
        /// </summary>
        public static ClaimsPrincipal GetClaimsAuthorized(JwtSecurityToken decodedJwtToken) => new ClaimsPrincipal(new ClaimsIdentity(decodedJwtToken.Claims.ToList(), "Admin"));
    }
}
