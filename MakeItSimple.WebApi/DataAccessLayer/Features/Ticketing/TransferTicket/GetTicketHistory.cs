using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTicketHistory.GetTicketHistoryResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class GetTicketHistory
    {
        public class GetTicketHistoryResult
        {
            public int ? RequestGeneratorId { get; set; }
            public string Requestor_Name { get; set; }
            public List<GetTicketHistoryConcern> GetTicketHistoryConcerns { get; set; }
            public class GetTicketHistoryConcern
            {
                public int TicketHistoryId { get; set; }
                public string Approver_Name { get; set; }
                public string Request { get; set; }
                public string Status { get; set; }
                public DateTime? Transaction_Date { get; set; }
            }
        }


        public class GetTicketHistoryQuery : IRequest<Result>
        {
            public int RequestGeneratorId { get; set; }
        }

        public class Handler : IRequestHandler<GetTicketHistoryQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetTicketHistoryQuery request, CancellationToken cancellationToken)
            {
                var requestGenerator = await _context.TicketHistories.Where(x => x.RequestGeneratorId == request.RequestGeneratorId)
                    .GroupBy(x => x.RequestGeneratorId).Select(x => new GetTicketHistoryResult
                    {
                        RequestGeneratorId = x.Key,
                        Requestor_Name = x.First().RequestorByUser.Fullname,
                        GetTicketHistoryConcerns = x.OrderByDescending(x => x.Id).Select(x => new GetTicketHistoryResult.GetTicketHistoryConcern
                        {
                            TicketHistoryId = x.Id,
                            Approver_Name = x.ApproverByUser.Fullname,
                            Request = x.Request,
                            Status = x.Status,
                            Transaction_Date = x.TransactionDate,

                        }).ToList()

                    }).ToListAsync();

                return Result.Success(requestGenerator);
            }
        }

    }
}
