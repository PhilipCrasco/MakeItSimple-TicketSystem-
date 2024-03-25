using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup
{
    public class UpdateSubUnitStatus
    {
        public class UpdateSubUnitStatusCommand : IRequest<Result>
        {
            public int? Id { get; set; }
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
                var subUnitExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.Id , cancellationToken);
                if (subUnitExist == null)
                {
                    return Result.Failure(SubUnitError.SubUnitNotExist());
                }

                subUnitExist.IsActive = !subUnitExist.IsActive;
                

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }

        }
    }
}
