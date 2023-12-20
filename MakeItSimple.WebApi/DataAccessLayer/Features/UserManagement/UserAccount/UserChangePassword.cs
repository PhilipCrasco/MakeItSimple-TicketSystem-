using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount
{
    public class UserChangePassword
    {
        public class UserChangePasswordResult
        {
            public Guid Id { get; set; }
            public string New_Password { get; set; }
            public bool? Is_PasswordChanged { get; set; }
        }

        public class UserChangePasswordCommand : IRequest<Result>
        {
            [Required]
            public Guid Id { get; set; }

            [Required]
            public string Old_Password { get; set; }
            [Required]
            public string New_Password { get; set; }
            [Required]
            public string Confirm_Password { get; set; }

        }


        public class Handler : IRequestHandler<UserChangePasswordCommand, Result>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }
            public async Task<Result> Handle(UserChangePasswordCommand command, CancellationToken cancellationToken)
            {
                var user = await  _context.Users.FirstOrDefaultAsync(x => x.Id == command.Id , cancellationToken);

                if (user == null)
                {
                    return Result.Failure(UserError.UserNotExist());
                }

                if(!BCrypt.Net.BCrypt.Verify(command.Old_Password , user.Password))
                {
                    return Result.Failure(UserError.UserOldPasswordInCorrect());

                }

                if(command.New_Password == user.Username)
                {
                    return Result.Failure(UserError.InvalidDefaultPassword());
                }

                user.Password = command.New_Password;
                user.IsPasswordChange = true;

                await _context.SaveChangesAsync(cancellationToken);

                var result = new UserChangePasswordResult
                {
                    Id = user.Id,
                    New_Password = user.Password,
                    Is_PasswordChanged = user.IsPasswordChange

                };

                return Result.Success(result);

            }
        }

    }
}
