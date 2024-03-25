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
            public int UserRoleId { get; set; }
            public int ? DepartmentId { get; set; }
            public int ? SubUnitId {  get; set; }
            public int? UnitId { get; set; }
            public int ? CompanyId { get; set; }
            public string LocationCode { get; set; }

            public int ? BusinessUnitId { get; set; }

            //public int ? TeamId { get; set; }

            public Guid ? Added_By { get; set; }

        }

        public class AddNewUserCommand : IRequest<Result>
        {

            public string EmpId { get; set; }
            public string Fullname { set; get; }
            public string Username { get; set; }
            public int UserRoleId { get; set; }
            public int ? DepartmentId { get; set; }
            public int ? SubUnitId { get; set; }

            //public int ? TeamId { get; set; }

            public int ? UnitId { get; set; }

            public int? CompanyId { get; set; }
            public string LocationCode { get; set; }

            public int? BusinessUnitId { get; set; }

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
             
                var UserRoleNotExist = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == command.UserRoleId , cancellationToken);

                if (UserRoleNotExist == null)
                {
                    return Result.Failure(UserError.UserRoleNotExist());
                }


                var CompanyNotExist = await _context.Companies.FirstOrDefaultAsync(x => x.Id == command.CompanyId, cancellationToken);

                if (CompanyNotExist == null)
                {
                    return Result.Failure(UserError.CompanyNotExist());
                }

                var BusinessUnitNotExist = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == command.BusinessUnitId, cancellationToken);

                if (BusinessUnitNotExist == null)
                {
                    return Result.Failure(UserError.BusinessUnitNotExist());
                }

                var DepartmentNotExist = await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.DepartmentId , cancellationToken);

                if(DepartmentNotExist == null)
                {
                    return Result.Failure(UserError.DepartmentNotExist());
                }


                var UnitNotExist = await _context.Units.FirstOrDefaultAsync(x => x.Id == command.UnitId, cancellationToken);
                if (UnitNotExist == null)
                {
                    return Result.Failure(UserError.UnitNotExist());
                }

                var SubUnitNotExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);

                if (SubUnitNotExist == null)
                {
                    return Result.Failure(UserError.SubUnitNotExist());
                }

                var LocationNotExist = await _context.Locations.FirstOrDefaultAsync(x => x.LocationCode == command.LocationCode, cancellationToken);

                if (LocationNotExist == null)
                {
                    return Result.Failure(UserError.LocationNotExist());
                }


                //if(command.TeamId != null)
                //{
                //    var teamNotExist = await _context.Teams.FirstOrDefaultAsync(x => x.Id == command.TeamId, cancellationToken);
                //    if (teamNotExist == null)
                //    {
                //        return Result.Failure(UserError.TeamNotExist());
                //    }

                //}

                var users = new User
                {

                    EmpId = command.EmpId, 
                    Fullname = command.Fullname,
                    Username = command.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(command.Username),
                    UserRoleId = command.UserRoleId,
                    SubUnitId = command.SubUnitId,
                    //TeamId = command.TeamId,
                    DepartmentId = command.DepartmentId,    
                    CompanyId = command.CompanyId,  
                    LocationId = LocationNotExist.Id,
                    BusinessUnitId = command.BusinessUnitId,  
                    UnitId = command.UnitId,
                    AddedBy = command.Added_By,
                    //ReceiverId = receiverExist.Id,
               
                };
                
                await _context.Users.AddAsync(users , cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var result = new AddNewUserResult
                {
                    Id = users.Id,
                    EmpId = users.EmpId,
                    Fullname = users.Fullname,
                    Username = users.Username,
                    UserRoleId = users.UserRoleId,
                    DepartmentId = users.DepartmentId,
                    SubUnitId= users.SubUnitId,
                    //TeamId = users.TeamId,  
                    CompanyId = users.CompanyId,
                    LocationCode = command.LocationCode,
                    BusinessUnitId = users.BusinessUnitId,
                    UnitId = users.UnitId,  
                    Added_By = users.AddedBy
                };

                return Result.Success(result);

            }
        }




    }
}
