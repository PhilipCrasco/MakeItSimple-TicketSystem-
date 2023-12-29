using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount
{
    public class UntagAndTagUserRolePermission
    {
        public class UntagAndTagUserRolePermissionResult
        {
            public int Id { get; set; }

            public ICollection<string> Permissions { get; set; }

            public string Message { get; set; }

            public Guid? Modified_By { get; set; }

            public DateTime? Updated_At { get; set; }
        }

        public class UntagAndTagUserRolePermissionCommand : IRequest<Result>
        {
            public int Id { get; set; }

            public ICollection<string> Permissions { get; set; }

            public Guid? Modified_By { get; set; }
        }


        public class Handler : IRequestHandler<UntagAndTagUserRolePermissionCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UntagAndTagUserRolePermissionCommand command, CancellationToken cancellationToken)
            {
                var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (userRole == null)
                {
                    return Result.Failure(UserRoleError.UserRoleNotExist());
                }

                string changeMessage = "Permission is Unchange";
                if(userRole.Permissions != null && command.Permissions != null)
                {
                    changeMessage = userRole.Permissions.Count < command.Permissions.Count ? "Permission has been successfully tagged" 
                         : userRole.Permissions.Count == command.Permissions.Count ? "Permission is Unchange" : "Permission has been successfully Untagged";
                }

                if(changeMessage != "Permission is Unchange")
                {
                    userRole.Permissions = command.Permissions;
                    userRole.UpdatedAt = DateTime.Now;
                    userRole.ModifiedBy = command.Modified_By;
                }

                await _context.SaveChangesAsync(cancellationToken);

                var result = new UntagAndTagUserRolePermissionResult
                {
                    Id = userRole.Id,
                    Permissions = userRole.Permissions,
                    Updated_At = DateTime.Now,
                    Modified_By = userRole.ModifiedBy,
                    Message = changeMessage

                };

                return Result.Success(result);


            }
        }


    }
}
