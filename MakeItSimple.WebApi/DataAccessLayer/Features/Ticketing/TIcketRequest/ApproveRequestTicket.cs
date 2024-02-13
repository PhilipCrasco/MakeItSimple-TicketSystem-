using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest
{
    public class ApproveRequestTicket
    {

        public class ApproveRequestTicketResult
        {
           public int ? RequestGeneratorId { get; set; }
            public List<Concerns> Concern { get; set; }
           public class Concerns
            {
                public int TicketConcernId { get; set; }
                public Guid ? Approved_By { get; set; }
                public DateTime ? Approved_At { get; set; }
                public bool ? IsApproved { get; set; }

            }
        }

        public class ApproveRequestTicketCommand : IRequest<Result>
        {
            public Guid ? Approved_By { get; set; }

            public List<Concerns> Concern { get; set; }

            public class Concerns
            {
              public int RequestGeneratorId { get; set; }
            }

        }

        public class Handler : IRequestHandler<ApproveRequestTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApproveRequestTicketCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;
                var ticketApproveList = new List<TicketConcern>();

                foreach (var ticketConcern in command.Concern)
                {
                    var requestGeneratorExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == ticketConcern.RequestGeneratorId, cancellationToken);
                    if (requestGeneratorExist == null)
                    {
                        return Result.Failure(TicketRequestError.TicketIdNotExist());
                    }

                    var ticketConcernExist = await _context.TicketConcerns.Where(x => x.RequestGeneratorId == ticketConcern.RequestGeneratorId).ToListAsync();

                    foreach(var concerns in ticketConcernExist)
                    {
                        if(concerns.TargetDate < dateToday)
                        {
                            return Result.Failure(TicketRequestError.DateTimeInvalid());
                        }

                        concerns.IsApprove = true;
                        concerns.ApprovedBy = command.Approved_By;
                        concerns.ApprovedAt = DateTime.Now;
                        concerns.IsReject = false;
                        concerns.Remarks = null; 
                        concerns.IsTransfer = null;
                        concerns.TransferBy = null;
                        concerns.TransferAt = null;

                        ticketApproveList.Add(concerns);
                    }

                }

                await _context.SaveChangesAsync(cancellationToken);

                var results = ticketApproveList.DistinctBy(x => x.RequestGeneratorId).Select(x => new ApproveRequestTicketResult
                {
                    RequestGeneratorId = x.RequestGeneratorId,
                    Concern = ticketApproveList.Select(x => new ApproveRequestTicketResult.Concerns
                    {
                        TicketConcernId = x.Id,
                        Approved_By = x.ApprovedBy,
                        Approved_At = x.ApprovedAt,
                        IsApproved = x.IsApprove

                    }).ToList(),

                });
               
                return Result.Success(results);
            }

        }
    }


}
