using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount
{
    public class AddNewUserRole
    {
        public class AddNewUserRoleResult
        {

            public int Id {  get; set; }

            public string User_Role_Name { get; set; }

            public ICollection<string> Permissions { get; set; }

            public Guid ? Added_By { get; set; }

            public DateTime Created_At { get; set; }

        }       
        

        public class AddNewUserRoleCommand : IRequest<Result>
        {
            public string User_Role_Name { get; set; }

            public ICollection<string> Permissions { get; set; }

            public Guid ? Added_By { get; set; }


        }


        public class Handler : IRequestHandler<AddNewUserRoleCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewUserRoleCommand command, CancellationToken cancellationToken)
            {
               
                var UserRoleAlreadyExist = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserRoleName == command.User_Role_Name , cancellationToken);
                
                if (UserRoleAlreadyExist != null)
                {
                    return Result.Failure(UserRoleError.UserRoleAlreadyExist(command.User_Role_Name));
                }

                var userRole = new UserRole
                {
                    UserRoleName = command.User_Role_Name,
                    Permissions = command.Permissions,
                    AddedBy = command.Added_By,
                    CreatedAt = DateTime.Now

                };

                await _context.UserRoles.AddAsync(userRole,cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var result = new AddNewUserRoleResult
                {
                    Id = userRole.Id,
                    User_Role_Name = userRole.UserRoleName,
                    Permissions = userRole.Permissions,
                    Added_By = userRole.AddedBy,
                    Created_At =  userRole.CreatedAt
                   
                };


                return Result.Success(result);


            }
        }





    }
}
