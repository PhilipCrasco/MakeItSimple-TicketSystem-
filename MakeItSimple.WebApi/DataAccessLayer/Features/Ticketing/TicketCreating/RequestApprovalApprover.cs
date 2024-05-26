using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApproveRequestTicket.ApproveRequestTicketCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class RequestApprovalApprover
    {

        public class RequestApprovalApproverCommand : IRequest<Result>
        {
            public Guid? Approved_By { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
            public int? RequestTransactionId { get; set; }
            public List<ApproverConcern> ApproverConcerns { get; set; }
            public class ApproverConcern
            {
                public Guid ? Issue_Handler { get; set; }
            }

        }

        public class Handler : IRequestHandler<RequestApprovalApproverCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RequestApprovalApproverCommand command, CancellationToken cancellationToken)
            {

                var dateToday = DateTime.Today;
                var ticketApproveList = new List<TicketConcern>();

                var requestTransactionExist = await _context.RequestTransactions
                        .FirstOrDefaultAsync(x => x.Id == command.RequestTransactionId,cancellationToken);

                if (requestTransactionExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                foreach (var ticketConcern in command.ApproverConcerns)
                {

                    var ticketConcernExist = await _context.TicketConcerns.
                        FirstOrDefaultAsync(x => x.RequestTransactionId == requestTransactionExist.Id
                        && x.UserId == ticketConcern.Issue_Handler
                        && x.IsApprove != true,cancellationToken);

                    if(ticketConcernExist is null)
                    {
                        return Result.Failure(TicketRequestError.TicketConcernIdNotExist());
                    }

                    var ticketConcernList = await _context.TicketConcerns
                        .Include(x => x.RequestorByUser)
                        .Where(x => x.RequestTransactionId == requestTransactionExist.Id 
                        && x.UserId == ticketConcern.Issue_Handler)
                        .ToListAsync();

                    var requestApproverList = await _context.ApproverTicketings
                     .Where(x => x.RequestTransactionId == requestTransactionExist.Id
                     && x.IsApprove == null && x.IssueHandler == ticketConcernExist.UserId)
                     .ToListAsync();


                    var selectRequestTicketId = requestApproverList
                      .FirstOrDefault(x => x.ApproverLevel == requestApproverList
                      .Min(x => x.ApproverLevel));


                    if (selectRequestTicketId is not null)
                    {

                        if (ticketConcernList.First().TicketApprover != command.UserId)
                        {
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                        }

                        selectRequestTicketId.IsApprove = true;

                        var userApprovalList = await _context.ApproverTicketings
                            .Include(x => x.User)
                            .Where(x => x.RequestTransactionId == selectRequestTicketId.RequestTransactionId)
                            .ToListAsync();

                        foreach (var concernTicket in ticketConcernList)
                        {

                            var validateUserApprover = userApprovalList.
                                FirstOrDefault(x => x.ApproverLevel == selectRequestTicketId.ApproverLevel + 1);

                            if (validateUserApprover != null)
                            {
                                concernTicket.TicketApprover = validateUserApprover.UserId;
                                concernTicket.ConcernStatus = $"{TicketingConString.RequestApproval} {validateUserApprover.User.Username}";
                            }
                            else
                            {
                                concernTicket.TicketApprover = null;
                                concernTicket.IsApprove = true;
                                concernTicket.ApprovedBy = command.Approved_By;
                                concernTicket.IsReject = false;

                            }

                        }


                    }


                }


                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
