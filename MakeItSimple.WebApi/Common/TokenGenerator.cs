using MakeItSimple.WebApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MakeItSimple.WebApi.Common
{
    public class TokenGenerator
    {
        private readonly IConfiguration _configuration;


        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user )
        {
            var key = _configuration.GetValue<string>("JwtConfig:Key");
            var audience = _configuration.GetValue<string>("JwtConfig:Audience");
            var issuer = _configuration.GetValue<string>("JwtConfig:Issuer");
            var KeyBytes = Encoding.ASCII.GetBytes(key);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("id", user.Id.ToString()),
                        new Claim(ClaimTypes.Name , user.Fullname),                  
                        new Claim(ClaimTypes.Email , user.Email),
                        new Claim(ClaimTypes.Role , user.UserRole.UserRoleName)
                    }),
                Expires = DateTime.UtcNow.AddDays(1),

                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(KeyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

     


    }
}
