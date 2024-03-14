using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup
{
    public class GetReceiverBusinessUnit
    {
        public class GetReceiverBusinessUnitResult
        {
            public int? BusinessUnitId { get; set; }
            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }

        }

        public class GetReceiverBusinessUnitQuery : IRequest<Result> { }

        public class Handler : IRequestHandler<GetReceiverBusinessUnitQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetReceiverBusinessUnitQuery request, CancellationToken cancellationToken)
            {

                var businessUnit = await _context.Receivers.ToListAsync();

                var selectBusinessUnit = businessUnit.Select(x => x.BusinessUnitId);

                var result = await _context.BusinessUnits
                    .Where(x => x.IsActive == true  && !selectBusinessUnit.Contains(x.Id))
                    .Select(x => new GetReceiverBusinessUnitResult
                    {
                        BusinessUnitId = x.Id,
                        BusinessUnit_Code = x.BusinessCode,
                        BusinessUnit_Name = x.BusinessName

                    }).ToListAsync();   

                return Result.Success(result);
            }
        }

    }
}
