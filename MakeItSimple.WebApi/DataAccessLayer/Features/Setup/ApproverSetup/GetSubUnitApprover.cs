using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup
{
    public class GetSubUnitApprover
    {

        public class GetSubUnitApproverResult
        {
            public int ? SubUnitId { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set;}
        }

        public class GetSubUnitApproverCommand : IRequest<Result>
        {
           
        }


        public class Handler : IRequestHandler<GetSubUnitApproverCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetSubUnitApproverCommand request, CancellationToken cancellationToken)
            {
                var approverList = await _context.Approvers
                    .Select(x => x.SubUnitId)
                    .ToListAsync();              

                var subUnitList = await _context.SubUnits
                    .Where(x => !approverList.Contains(x.Id) && x.IsActive == true)
                    .Select(x => new GetSubUnitApproverResult
                {
                    SubUnitId = x.Id,
                    SubUnit_Code = x.SubUnitCode,
                    SubUnit_Name = x.SubUnitName

                }).ToListAsync();

                return Result.Success(subUnitList);
            }
        }
    }
}
