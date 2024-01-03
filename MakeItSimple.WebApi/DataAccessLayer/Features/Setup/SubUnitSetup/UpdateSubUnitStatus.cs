using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup
{
    public class UpdateSubUnitStatus
    {
        public class UpdateSubUnitStatusResult
        {
            public int Id { get; set; }
            public bool Status { get; set; }
        }

        public class UpdateSubUnitStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public bool Status { get; set; }
        }


        public class Handler : IRequestHandler<UpdateSubUnitStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateSubUnitStatusCommand command, CancellationToken cancellationToken)
            {
                var subUnit = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (subUnit == null)
                {
                    return Result.Failure(SubUnitError.SubUnitNotExist());
                }

                var subUnitIsUse = await _context.Channels.AnyAsync(x => x.SubUnitId == command.Id && x.IsActive == true, cancellationToken);

                if (subUnitIsUse == true)
                {
                    return Result.Failure(SubUnitError.SubUnitIsUse(subUnit.SubUnitName));
                }

                subUnit.IsActive = !subUnit.IsActive;

                await _context.SaveChangesAsync(cancellationToken); 

                var results = new UpdateSubUnitStatusResult
                {
                    Id = subUnit.Id,
                    Status = subUnit.IsActive
                };


                return Result.Success(results);

            }
        }
    }
}
