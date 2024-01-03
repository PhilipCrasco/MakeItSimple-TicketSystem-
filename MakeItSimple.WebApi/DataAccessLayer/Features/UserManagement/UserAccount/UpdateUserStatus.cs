using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount
{
    public class UpdateUserStatus
    {
        
        public class UpdateUserStatusResult
        {
            public Guid Id { get; set; }
            public  bool Status { get; set; }
        }

        public class UpdateUserStatusCommand : IRequest<Result>
        {
            public Guid Id { get; set; }

        }

        public class Handler : IRequestHandler<UpdateUserStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateUserStatusCommand command, CancellationToken cancellationToken)
            {
                
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id  == command.Id , cancellationToken );

                if (user == null)
                {
                    return Result.Failure(UserError.UserNotExist());
                }
                var userIsUse = await _context.ChannelUsers.AnyAsync(x => x.UserId == command.Id, cancellationToken);
                if (userIsUse == true)
                {
                    return Result.Failure(UserError.UserIsUse(user.Fullname));
                }

                user.IsActive = !user.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new UpdateUserStatusResult
                {
                    Id = user.Id,
                    Status = user.IsActive
                };

                return Result.Success(results);



            }
        }
    }

}
