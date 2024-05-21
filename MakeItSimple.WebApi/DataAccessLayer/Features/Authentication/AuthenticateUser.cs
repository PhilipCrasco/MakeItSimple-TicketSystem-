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

            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string UserRoleName { get; set; }
            //public string CompanyName {  get; set; }  
            //public string BusinessName { get; set; }
            //public string DepartmentName { get; set; }
            //public string UnitName {  get; set; }
            //public string SubUnitName { get; set; }
            //public string LocationName { get; set; }
            public ICollection<string> Permissions { get; set; }
            public string Token { get; set; }
            public bool ? IsPasswordChanged { get; set; }
            public string Message { get; set; }


            public AuthenticateUserResult(User user , string token)
            {
                Id = user.Id;
                EmpId = user.EmpId;
                Fullname = user.Fullname;
                Username = user.Username;
                //CompanyName = user.Company.CompanyName;
                //BusinessName = user.BusinessUnit.BusinessName;
                //DepartmentName = user.Department.DepartmentName;
                //UnitName = user.Units.UnitName;
                //SubUnitName = user.SubUnit.SubUnitName;
                //LocationName = user.Location.LocationName;
                UserRoleName = user.UserRole.UserRoleName;
                Permissions = user.UserRole?.Permissions;
                IsPasswordChanged = user.IsPasswordChange;
                Token = token;
                Message = "Yehey";
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
            private readonly TokenGenerator _tokenGenerator;

            public Handler(MisDbContext context , TokenGenerator tokenGenerator)
            {
                _context = context;
                _tokenGenerator = tokenGenerator;
            }

            public async Task<Result> Handle(AuthenticateUserQuery command, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(x => x.UserRole)
                    .Include(x => x.Company)
                    .Include(x => x.BusinessUnit)
                    .Include(x => x.Department)
                    .Include(x => x.Units)
                    .Include(x => x.SubUnit)
                    .Include(x => x.Location)
                    .SingleOrDefaultAsync(x => x.Username == command.UsernameOrEmail);

                if(user == null || !BCrypt.Net.BCrypt.Verify(command.Password , user.Password))
                {
                    return Result.Failure(AuthenticationError.UsernameAndPasswordIncorrect());
                }

                if(user.IsActive != true)
                {
                  return Result.Failure(AuthenticationError.UserNotActive()); 
                }

                 
                if(user.UserRoleId == null)
                {
                   return Result.Failure(AuthenticationError.NoRole());
                }

                await _context.SaveChangesAsync(cancellationToken);

                var token = _tokenGenerator.GenerateJwtToken(user );

                var results = user.ToGetAuthenticatedUserResult(token);

                return Result.Success(results);

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
