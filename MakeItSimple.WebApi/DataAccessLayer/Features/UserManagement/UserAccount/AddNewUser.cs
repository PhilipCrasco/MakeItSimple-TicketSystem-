﻿using MakeItSimple.WebApi.DataAccessLayer.Data;
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
            public int ? LocationId { get; set; }

            public int ? BusinessUnitId { get; set; }

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

            public int ? UnitId { get; set; }

            public int? CompanyId { get; set; }
            public int? LocationId { get; set; }

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

                var DepartmentNotExist = await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.DepartmentId , cancellationToken);

                if(DepartmentNotExist == null)
                {
                    return Result.Failure(UserError.DepartmentNotExist());
                }

                var SubUnitNotExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);

                if (SubUnitNotExist == null)
                {
                    return Result.Failure(UserError.SubUnitNotExist());
                }

                var CompanyNotExist = await _context.Companies.FirstOrDefaultAsync(x => x.Id == command.CompanyId, cancellationToken);

                if (CompanyNotExist == null)
                {
                    return Result.Failure(UserError.CompanyNotExist());
                }
                var LocationNotExist = await _context.Locations.FirstOrDefaultAsync(x => x.Id == command.LocationId, cancellationToken);

                if (LocationNotExist == null)
                {
                    return Result.Failure(UserError.LocationNotExist());
                }
                var BusinessUnitNotExist = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == command.BusinessUnitId, cancellationToken);

                if (BusinessUnitNotExist == null)
                {
                    return Result.Failure(UserError.BusinessUnitNotExist());
                }

                //var receiverExist = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == BusinessUnitNotExist.Id , cancellationToken);
                //if (receiverExist == null)
                //{
                //    return Result.Failure(UserError.ReceiverNotExist());
                //}

                var UnitNotExist = await _context.Units.FirstOrDefaultAsync(x => x.Id == command.UnitId, cancellationToken);
                if (UnitNotExist == null)
                {
                    return Result.Failure(UserError.UnitNotExist());
                }


                var users = new User
                {

                    EmpId = command.EmpId, 
                    Fullname = command.Fullname,
                    Username = command.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(command.Username),
                    UserRoleId = command.UserRoleId,
                    SubUnitId = command.SubUnitId,
                    DepartmentId = command.DepartmentId,    
                    CompanyId = command.CompanyId,  
                    LocationId = command.LocationId,
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
                    CompanyId = users.CompanyId,
                    LocationId = users.LocationId,
                    BusinessUnitId = users.BusinessUnitId,
                    UnitId = users.UnitId,  
                    Added_By = users.AddedBy
                };

                return Result.Success(result);

            }
        }




    }
}
