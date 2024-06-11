using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetTicketHistory.GetTicketHistoryResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern
{
    public class GetTicketHistory
    {
        public class GetTicketHistoryResult
        {
            public int? TicketConcernId { get; set; }
            public List<GetTicketHistoryConcern> GetTicketHistoryConcerns { get; set; }
            public class GetTicketHistoryConcern
            {
                public int TicketHistoryId { get; set; }
                public string Request { get; set; }
                public string Status { get; set; }
                public string Transacted_By { get; set; }
                public DateTime? Transaction_Date { get; set; }


            }
        }


        public class GetTicketHistoryQuery : IRequest<Result>
        {
            public int TicketConcernId { get; set; }
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
                var ticketHistory = await _context.TicketHistories
                    .Include(x => x.TransactedByUser)
                    .Where(x => x.TicketConcernId == request.TicketConcernId)
                    .GroupBy(x => x.TicketConcernId).Select(x => new GetTicketHistoryResult
                    {
                       TicketConcernId = x.Key,
                        GetTicketHistoryConcerns = x.OrderByDescending(x => x.Id)
                        .Select(x => new GetTicketHistoryConcern
                        {
                            TicketHistoryId = x.Id,
                            Transacted_By = x.TransactedByUser.Fullname,
                            Request = x.Request,
                            Status = x.Status,
                            Transaction_Date = x.TransactionDate,

                        }).ToList()

                    }).ToListAsync();

                return Result.Success(/*requestGenerator*/);
            }
        }

    }
}
