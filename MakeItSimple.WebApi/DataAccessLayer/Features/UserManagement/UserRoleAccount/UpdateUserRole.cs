using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount
{
    public class UpdateUserRole
    {
        public class UpdateUserRoleResult
        {
            public int Id { get; set; }

            public string UserRoleName { get; set; }

            public Guid ? Modified_By { get; set; }

            public DateTime ? Updated_At { get; set; }


        }

        public class UpdateUserRoleCommand : IRequest<Result>
        {
            public int Id { get; set; }

            public string User_Role_Name { get; set; }

            public Guid ? Modified_By { get;  set; }
        }

        public class Handler : IRequestHandler<UpdateUserRoleCommand, Result>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                 _context = context;
            }
            public async Task<Result> Handle(UpdateUserRoleCommand command, CancellationToken cancellationToken)
            {

                var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (userRole == null)
                {
                    return Result.Failure(UserRoleError.UserRoleNotExist());
                }
                else if (userRole.UserRoleName == command.User_Role_Name)
                {
                    return Result.Failure(UserRoleError.UserRoleNoChanges());
                }

                var UserRoleAlreadyExist = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserRoleName == command.User_Role_Name, cancellationToken);

                if (UserRoleAlreadyExist != null && userRole.UserRoleName != command.User_Role_Name)
                {
                    return Result.Failure(UserRoleError.UserRoleAlreadyExist(command.User_Role_Name));
                }


                var UserRoleIsUse = await _context.Users.AnyAsync(x => x.UserRoleId == command.Id && x.IsActive == true, cancellationToken);

                if (UserRoleIsUse == true)
                {
                    return Result.Failure(UserRoleError.UserRoleIsUse(userRole.UserRoleName));
                }

                userRole.UserRoleName = command.User_Role_Name;
                userRole.ModifiedBy = command.Modified_By;
                userRole.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);

                var result = new UpdateUserRoleResult
                {
                    Id = userRole.Id,
                    UserRoleName = userRole.UserRoleName,
                    Modified_By = userRole.ModifiedBy,
                    Updated_At = userRole.UpdatedAt
                    
                };

                return Result.Success(result);

            }
        }



    }
}
