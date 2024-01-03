using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Drawing.Printing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup
{
    public class UpdateSubUnit
    {

        public class UpdateSubUnitResult
        {
            public int Id { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public int? DepartmentId { get; set; }
            public Guid? Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }
        };

        public class UpdateSubUnitCommand :IRequest<Result>
        {
            public int Id { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public int? DepartmentId { get; set; }
            public Guid? Modified_By { get; set; }

        }

        public class Handler : IRequestHandler<UpdateSubUnitCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateSubUnitCommand command, CancellationToken cancellationToken)
            {

                var subUnit = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (subUnit == null)
                {
                    return Result.Failure(SubUnitError.SubUnitNotExist());
                }
                else if (subUnit.SubUnitCode == command.SubUnit_Code && subUnit.SubUnitName == command.SubUnit_Name && subUnit.DepartmentId == command.DepartmentId)
                {
                    return Result.Failure(SubUnitError.SubUnitNochanges());
                }

                var subUnitCodeAlreadyExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitCode == command.SubUnit_Code, cancellationToken);

                if (subUnitCodeAlreadyExist != null && subUnit.SubUnitCode != command.SubUnit_Code)
                {
                    return Result.Failure(SubUnitError.SubUnitCodeAlreadyExist(command.SubUnit_Code));
                }

                var subUnitNameAlreadyExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitName == command.SubUnit_Name, cancellationToken);

                if (subUnitNameAlreadyExist != null && subUnit.SubUnitName != command.SubUnit_Name)
                {
                    return Result.Failure(SubUnitError.SubUnitNameAlreadyExist(command.SubUnit_Name));
                }

                var departmentNotExist = await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.DepartmentId, cancellationToken);

                if (departmentNotExist == null)
                {
                    return Result.Failure(SubUnitError.DepartmentNotExist());
                }

                var subUnitIsUse = await _context.Channels.AnyAsync(x => x.SubUnitId == command.Id && x.IsActive == true, cancellationToken);

                if (subUnitIsUse == true)
                {
                    return Result.Failure(SubUnitError.SubUnitIsUse(subUnit.SubUnitName));
                }

                subUnit.SubUnitCode = command.SubUnit_Code;
                subUnit.SubUnitName = command.SubUnit_Name;
                subUnit.DepartmentId = command.DepartmentId;
                subUnit.ModifiedBy = command.Modified_By;
                subUnit.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new UpdateSubUnitResult
                {
                    Id = subUnit.Id,
                    SubUnit_Code = subUnit.SubUnitCode,
                    SubUnit_Name = subUnit.SubUnitName, 
                    DepartmentId = subUnit.DepartmentId,
                    Modified_By = subUnit.ModifiedBy,
                    Updated_At = subUnit.UpdatedAt
                };

                return Result.Success(results);

            }
        }
    }
}
