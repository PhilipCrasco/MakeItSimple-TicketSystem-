using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures
{
    public class AddNewUser
    {

        public class AddNewUserResult
        {   
            public Guid Id {  get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public int UserRoleId { get; set; }
            public int ? DepartmentId { get; set; }
            public Guid ? Added_By { get; set; }

        }

        public class AddNewUserCommand : IRequest<Result>
        {
            public Guid Id { get; set; }
            public string EmpId { get; set; }
            public string Fullname { set; get; }
            public string Username { get; set; }
            public string Email { get; set; }
            public int UserRoleId { get; set; }
            public int ? DepartmentId { get; set; }
            public Guid ? Added_By { get; set; }

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

                var UserAlreadyExist = await _context.Users.FirstOrDefaultAsync(x =>  x.EmpId == command.EmpId && x.Fullname == command.Fullname , cancellationToken);

                if (UserAlreadyExist != null)
                {
                    return Result.Failure(UserError.UserAlreadyExist(command.EmpId , command.Fullname));
                }

                var UsernameAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.Username == command.Username , cancellationToken);
                if (UsernameAlreadyExist != null)
                {
                    return Result.Failure(UserError.UsernameAlreadyExist(command.Username));
                }

                var EmailAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email, cancellationToken);

                if(EmailAlreadyExist != null)
                {
                    return Result.Failure(UserError.EmailAlreadyExist(command.Email));
                }
                
                var UserRoleNotExist = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == command.UserRoleId , cancellationToken);

                if (UserRoleNotExist == null)
                {
                    return Result.Failure(UserError.UserRoleNotExist());
                }

                var DepartmentNotExist = await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.DepartmentId , cancellationToken);

                if(DepartmentNotExist == null)
                {
                    return Result.Failure(UserError.DepartmentNotExist());
                }
               

                var users = new User
                {

                    EmpId = command.EmpId, 
                    Fullname = command.Fullname,
                    Username = command.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(command.Username),
                    Email = command.Email,
                    UserRoleId = command.UserRoleId,
                    DepartmentId = command.DepartmentId,    
                    AddedBy = command.Added_By,
                    
                };
                
                await _context.Users.AddAsync(users , cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var result = new AddNewUserResult
                {
                    Id = users.Id,
                    EmpId = users.EmpId,
                    Fullname = users.Fullname,
                    Username = users.Username,
                    Email = users.Email,
                    UserRoleId = users.UserRoleId,
                    DepartmentId = users.DepartmentId,
                    Added_By = users.AddedBy
                };

                return Result.Success(result);

            }
        }




    }
}
