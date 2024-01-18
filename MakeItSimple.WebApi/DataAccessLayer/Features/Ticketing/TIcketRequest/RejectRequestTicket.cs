using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.RejectRequestTicket.RejectRequestTicketCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest
{
    public class RejectRequestTicket
    {

        public class RejectRequestTicketResult
        {
            public int ? RequestGeneratorId { get; set; }

            public List<RejectConcern> RejectConcerns { get; set; }

            public class RejectConcern
            {
                public int TicketConcernId { get; set; }
                public string Reject_Remarks { get; set; }
                public bool Is_Reject { get; set; }

            }

        }


        public class RejectRequestTicketCommand : IRequest<Result>
        {
            public string Reject_Remarks { get; set; }

            public List<RejectConcern> RejectConcerns { get; set; }

            public class RejectConcern
            {
                public int RequestGeneratorId { get; set; }
            }
        }


        public class Handler : IRequestHandler<RejectRequestTicketCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectRequestTicketCommand command, CancellationToken cancellationToken)
            {
                var ticketApproveList = new List<TicketConcern>();

                foreach (var ticketConcern in command.RejectConcerns)
                {
                    var requestGeneratorExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == ticketConcern.RequestGeneratorId, cancellationToken);
                    if (requestGeneratorExist == null)
                    {
                        return Result.Failure(TicketRequestError.TicketIdNotExist());
                    }

                    var ticketConcernExist = await _context.TicketConcerns.Where(x => x.RequestGeneratorId == ticketConcern.RequestGeneratorId).ToListAsync();

                    foreach (var concerns in ticketConcernExist)
                    {
                        concerns.IsReject = true;
                        concerns.RejectRemarks = command.Reject_Remarks;

                        ticketApproveList.Add(concerns);
                    }


                }

                await _context.SaveChangesAsync(cancellationToken);

                var results = ticketApproveList.DistinctBy(x => x.RequestGeneratorId).Select(x => new RejectRequestTicketResult
                {
                    RequestGeneratorId = x.RequestGeneratorId,
                    RejectConcerns = ticketApproveList.Select(x => new RejectRequestTicketResult.RejectConcern
                    {
                        TicketConcernId = x.Id,
                        Reject_Remarks = x.RejectRemarks,
                        Is_Reject = x.IsReject,

                    }).ToList(),

                });

                return Result.Success(results);
            }
        }
    }
}
