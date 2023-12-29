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
            public string Email { get; set; }
            public int UserRoleId { get; set; }

            public int ? DepartmentId { get; set; }

            public int ? SubUnitId {  get; set; }

            public Guid ? Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }

        }

        public class UpdateUserCommand : IRequest<Result>
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public int UserRoleId { get; set; }
            public int? DepartmentId { get; set; }

            public int ? SubUnitId { get; set; }

            public Guid? Modified_By { get; set; }
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

                var UserRoleNotExist = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == command.UserRoleId , cancellationToken);

                if (UserRoleNotExist == null)
                {
                    return Result.Failure(UserError.UserRoleNotExist());
                }

                var DepartmentNotExist = await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.DepartmentId, cancellationToken);

                if (DepartmentNotExist == null)
                {
                    return Result.Failure(UserError.DepartmentNotExist());
                }

                var SubUnitNotExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);

                if (SubUnitNotExist == null)
                {
                    return Result.Failure(UserError.SubUnitNotExist());
                }


                User.Email = command.Email;
                User.UserRoleId = command.UserRoleId;
                User.DepartmentId = command.DepartmentId;
                User.SubUnitId = command.SubUnitId;
                User.UpdatedAt = DateTime.Now;
                User.ModifiedBy = command.Modified_By;

                await _context.SaveChangesAsync(cancellationToken);

                var result = new UpdateUserResult
                {
                    Id = command.Id,    
                    Email = User.Email,
                    UserRoleId = User.UserRoleId,
                    DepartmentId = User.DepartmentId,
                    SubUnitId = User.SubUnitId,
                    Updated_At = User.UpdatedAt,
                    Modified_By = User.ModifiedBy,
                };

                return Result.Success(result);




            }
        }



    }
}
