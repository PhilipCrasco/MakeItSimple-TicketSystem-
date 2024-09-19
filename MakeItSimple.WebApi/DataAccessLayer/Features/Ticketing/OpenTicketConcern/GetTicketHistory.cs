using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
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
                public string Remarks { get; set; }
                public int ? Approver_Level { get; set; }
                public bool ? IsApproved { get; set; }
            }

            public List<UpComingApprover> UpComingApprovers { get; set; }
            public class UpComingApprover
            {
                public int TicketHistoryId { get; set; }
                public string Request { get; set; }
                public string Status { get; set; }
                public string Transacted_By { get; set; }
                public DateTime? Transaction_Date { get; set; }
                public string Remarks { get; set; }
                public int? Approver_Level { get; set; }
                public bool ? IsApproved { get; set; }
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
                    .AsNoTracking()
                    .Include(x => x.TransactedByUser)
                    .Include(x => x.TicketConcern)
                    .Where(x => x.TicketConcernId == request.TicketConcernId)
                    .GroupBy(x => x.TicketConcernId).Select(x => new GetTicketHistoryResult
                    {
                        TicketConcernId = x.Key,
                        GetTicketHistoryConcerns = x.OrderByDescending(x => x.Id)
                        .Where(x => !x.Request.Contains(TicketingConString.Approval) && !x.Request.Contains(TicketingConString.NotConfirm))
                        .Select(x => new GetTicketHistoryConcern
                        {
                            TicketHistoryId = x.Id,
                            Transacted_By = x.TransactedByUser.Fullname,
                            Request = x.Request,
                            Status = x.Status,
                            Transaction_Date = x.TransactionDate,
                            Remarks = x.Remarks,
                            Approver_Level = x.Approver_Level,
                            IsApproved = x.IsApprove,

                        }).ToList(),

                        UpComingApprovers = x.OrderBy(x => x.Id)
                        .OrderByDescending(x => x.Id)
                        .Where(x => (x.IsApprove == null && x.Approver_Level != null) || x.Request.Contains(TicketingConString.Approval)
                        || x.Request.Contains(TicketingConString.NotConfirm)) 
                        .Select(x => new UpComingApprover
                        {
                            TicketHistoryId = x.Id,
                            Transacted_By = x.TransactedByUser.Fullname,
                            Request = x.Request,
                            Status = x.Status,
                            Transaction_Date = x.TransactionDate,
                            Remarks = x.Remarks,
                            Approver_Level = x.Approver_Level,
                            IsApproved = x.IsApprove,
                                                                               
                        }).ToList(),

                    }).ToListAsync();
                    
                return Result.Success(ticketHistory);
            }

        }

    }
}
