using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures
{
    public class UpdateUser
    {

        public class UpdateUserResult
        {
            public Guid Id {  get; set; }
            public string Username { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public int UserRoleId { get; set; }
            public Guid ? ModifiedBy { get; set; }
            public DateTime Updated_At { get; set; }

        }

        public class UpdateUserCommand : IRequest<Result>
        {
            public Guid Id { get; set; }
            public string Username { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public int UserRoleId { get; set; }       
            public Guid? ModifiedBy { get; set; }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, Result>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
            {

                var User = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.Id , cancellationToken);

                if (User == null)
                {
                    return Result.Failure(UserError.UserNotExist());

                }

                var UserAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.Fullname == command.Fullname , cancellationToken);

                if (UserAlreadyExist != null && UserAlreadyExist.Fullname != User.Fullname)
                {
                    return Result.Failure(UserError.UserAlreadyExist(command.Fullname));
                }

                var UsernameAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.Username == command.Username, cancellationToken);
                if (UsernameAlreadyExist != null)
                {
                    return Result.Failure(UserError.UsernameAlreadyExist(command.Username));
                }

                var UserRoleNotExist = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == command.UserRoleId , cancellationToken);

                if (UserRoleNotExist == null)
                {
                    return Result.Failure(UserError.UserRoleNotExist());
                }

                User.Fullname = command.Fullname;
                User.Username = command.Username;
                User.Email = command.Email;
                User.UserRoleId = command.UserRoleId;
                User.UpdatedAt = DateTime.Now;
                User.ModifiedBy = command.ModifiedBy;

                await _context.SaveChangesAsync(cancellationToken);

                var result = new UpdateUserResult
                {
                    Id = command.Id,    
                    Fullname = User.Fullname,
                    Username = User.Username,
                    Email = User.Email,
                    UserRoleId = User.UserRoleId,
                    Updated_At = User.UpdatedAt,
                    ModifiedBy = User.ModifiedBy,
                };

                return Result.Success(result);




            }
        }



    }
}
