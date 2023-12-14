using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models;

namespace MakeItSimple.DataAccessLayer.Features.UserFeatures
{
    public class AddNewUser
    {

        public class AddNewUserResult
        {   
            public Guid Id {  get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            
        }

        public class AddNewUserCommand : IRequest<Result>
        {
            public Guid Id { get; set; }
            public string Fullname { set; get; }
            public string Username { get; set; }
            public string Password { get; set; }

            public string Email { get; set; }

        }


        public class Handler : IRequestHandler<AddNewUserCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewUserCommand command, CancellationToken cancellationToken)
            {

                var UserAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.Fullname == command.Fullname);

                if (UserAlreadyExist != null)
                {
                    return Result.Failure(UserError.UserAlreadyExist(command.Fullname));
                }


                var users = new User
                {
                    Fullname = command.Fullname,
                    Username = command.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(command.Password),
                    Email = command.Email,
                };
                
                await _context.Users.AddAsync(users , cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);


                var result = new AddNewUserResult
                {
                    Id = users.Id,
                    Fullname = users.Fullname,
                    Username = users.Username,
                    Password = users.Password,
                    Email = users.Email,

                };

                return Result.Success(result);

            }
        }




    }
}
