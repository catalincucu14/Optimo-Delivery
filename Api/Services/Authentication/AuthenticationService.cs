using Api.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Resources;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApiConfiguration _apiConfiguration;

        public AuthenticationService(ApiConfiguration apiConfiguration) => _apiConfiguration = apiConfiguration;

        /// <inheritdoc />
        public string GenerateJwtToken(List<Claim> claims)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();

            byte[] key = Encoding.ASCII.GetBytes(_apiConfiguration.JwtSecret);

            // Set the configuration of the JWT
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(_apiConfiguration.JwtLifeSpan),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Generate the JWT
            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }

        /// <inheritdoc />
        public List<Claim> GetClaimsAdmin(Admin admin) => new List<Claim>
        {
            new Claim("Id", admin.AdminId.ToString()),
            new Claim("Mail", admin.Mail),
            new Claim("Phone", admin.Phone),
            new Claim("Country", admin.Country),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        /// <inheritdoc />
        public List<Claim> GetClaimsDriver(Driver driver) => new List<Claim>
        {
            new Claim("Id", driver.DriverId.ToString()),
            new Claim("AdminId", driver.AdminId.ToString()),
            new Claim("Mail", driver.Mail),
            new Claim("Name", driver.Name),
            new Claim(ClaimTypes.Role, "Driver"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
    }
}
