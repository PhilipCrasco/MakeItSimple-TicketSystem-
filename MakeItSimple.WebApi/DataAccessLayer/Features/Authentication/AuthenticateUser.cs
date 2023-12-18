using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.AuthenticationFeatures
{
    public class AuthenticateUser
    {
        public class AuthenticateUserResult
        {
            public Guid Id { get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string UserRoleName { get; set; }
            public ICollection<string> Permissions { get; set; }
            public string Token { get; set; }
            public bool ? IsPasswordChanged { get; set; }



            public AuthenticateUserResult(User user , string token)
            {
                Id = user.Id;
                Fullname = user.Fullname;
                Username = user.Username;
                Email = user.Email;
                UserRoleName = user.UserRole.UserRoleName;
                Permissions = user.UserRole?.Permissions;
                IsPasswordChanged = user.IsPasswordChange;
                Token = token;
                
            }

        }

        public class AuthenticateUserQuery : IRequest<Result>
        {
            [Required]
            public string UsernameOrEmail { get; set; }
            [Required]
            public string Password { get; set; }

            public AuthenticateUserQuery(string usernameOrEmail)
            {
                UsernameOrEmail = usernameOrEmail;
            }

        }


        public class Handler : IRequestHandler<AuthenticateUserQuery, Result>
        {

            private readonly MisDbContext _context;
            private readonly IConfiguration _configuration;

            public Handler(MisDbContext context , IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<Result> Handle(AuthenticateUserQuery command, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(x => x.UserRole)
                    .SingleOrDefaultAsync(x => x.Username == command.UsernameOrEmail && x.IsActive == true|| x.Email == command.UsernameOrEmail && x.IsActive == true);

                if(user == null || !BCrypt.Net.BCrypt.Verify(command.Password , user.Password))
                {
                    return Result.Failure(AuthenticationError.UsernameAndPasswordIncorrect());
                }

                if(user.UserRoleId == null)
                {
                   return Result.Failure(AuthenticationError.NoRole());
                }

                await _context.SaveChangesAsync(cancellationToken);

                var token = GenerateJwtToken(user);

                var results = user.ToGetAuthenticatedUserResult(token);

                return Result.Success(results);

            }


            private string GenerateJwtToken(User user)
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

    public static class AuthenticateMappingExtension
    {
        public static AuthenticateUser.AuthenticateUserResult ToGetAuthenticatedUserResult(this User user, string token)
        {
            return new AuthenticateUser.AuthenticateUserResult(user, token);
        }

    }

}
