using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup
{
    public class AddNewSubUnit
    {

        public class AddNewSubUnitResult
        {
            public int Id { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public int ? DepartmentId { get; set; }
            public Guid ? Added_By { get; set; }
            public DateTime Created_At { get; set; }
        }


        public class AddNewSubUnitCommand : IRequest<Result>
        {
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public int ? DepartmentId { get; set; }
            public Guid? Added_By { get; set; }
        }


        public class Handler : IRequestHandler<AddNewSubUnitCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewSubUnitCommand command, CancellationToken cancellationToken)
            {

                var SubUnitCodeAlreadyExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitCode == command.SubUnit_Code, cancellationToken);

                if (SubUnitCodeAlreadyExist != null)
                {
                    return Result.Failure(SubUnitError.SubUnitCodeAlreadyExist(command.SubUnit_Code));
                }

                var SubUnitNameAlreadyExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitName == command.SubUnit_Name, cancellationToken);

                if (SubUnitNameAlreadyExist != null)
                {
                    return Result.Failure(SubUnitError.SubUnitNameAlreadyExist(command.SubUnit_Name));
                }

                var DepartmentNotExist = await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.DepartmentId, cancellationToken);

                if(DepartmentNotExist == null)
                {
                    return Result.Failure(SubUnitError.DepartmentNotExist());
                }

                var subUnit = new SubUnit
                {
                    SubUnitCode = command.SubUnit_Code,
                    SubUnitName = command.SubUnit_Name,
                    DepartmentId = command.DepartmentId,
                    AddedBy = command.Added_By,
                };

                await _context.SubUnits.AddAsync(subUnit, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

              
                var results = new AddNewSubUnitResult
                {
                    Id = subUnit.Id,
                    SubUnit_Code = subUnit.SubUnitCode,
                    SubUnit_Name = subUnit.SubUnitName,
                    DepartmentId = subUnit.DepartmentId,
                    Created_At = subUnit.CreatedAt,
                    Added_By = subUnit.AddedBy,
                };

                return Result.Success(results);

            }
        }
    }
}
