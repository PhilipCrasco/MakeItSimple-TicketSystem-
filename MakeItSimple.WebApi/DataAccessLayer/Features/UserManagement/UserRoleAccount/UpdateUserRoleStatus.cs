using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount
{
    public class UpdateUserRoleStatus
    {

        public class UpdateUserRoleStatusResult
        {
            public int Id { get; set; }
            public bool Status { get; set; }  
        }

        public class UpdateUserRoleStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<UpdateUserRoleStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateUserRoleStatusCommand command, CancellationToken cancellationToken)
            {


                var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == command.Id , cancellationToken);

                if (userRole == null)
                {
                    return Result.Failure(UserRoleError.UserRoleNotExist());
                }

                var UserRoleIsUse = await _context.Users.AnyAsync(x => x.UserRoleId == command.Id && x.IsActive == true, cancellationToken);

                if (UserRoleIsUse == true)
                {
                    return Result.Failure(UserRoleError.UserRoleIsUse(userRole.UserRoleName));
                }

                userRole.IsActive = !userRole.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                var result = new UpdateUserRoleStatusResult
                {
                    Id = userRole.Id,
                    Status = userRole.IsActive
                };


                return Result.Success(result);


            }
        }
    }
}
