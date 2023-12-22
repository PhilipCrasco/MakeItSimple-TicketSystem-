using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount
{
    public class UserResetPassword
    {
        public class UserResetPasswordResult
        {
            public Guid Id { get; set; }
            public bool ? IsPasswordChange { get; set; }
        }

        public class UserResetPasswordCommand : IRequest<Result>
        {

            public Guid ? Id { get; set; }

        }

        public class Handler : IRequestHandler<UserResetPasswordCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UserResetPasswordCommand command, CancellationToken cancellationToken)
            {

                var User = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (User == null)
                {
                    return Result.Failure(UserError.UserNotExist());

                }

                User.Password = BCrypt.Net.BCrypt.HashPassword(User.Username);
                User.IsPasswordChange = null;
                
               await _context.SaveChangesAsync(cancellationToken);

                var results = new UserResetPasswordResult
                {
                    Id = User.Id,
                    IsPasswordChange = User.IsPasswordChange
                };

                return Result.Success(results);

            }




        }




    }


}
