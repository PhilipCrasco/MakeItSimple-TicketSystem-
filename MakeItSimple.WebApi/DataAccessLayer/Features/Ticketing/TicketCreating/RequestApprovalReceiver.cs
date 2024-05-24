using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApproveRequestTicket.ApproveRequestTicketCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class RequestApprovalReceiver
    {

        public class RequestApprovalReceiverCommand : IRequest<Result>
        {
            public Guid? Approved_By { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
            public int? RequestTransactionId { get; set; }
            public List<Concern> Concerns { get; set; }

            public  class Concern
            {
                public Guid? IssueHandler { get; set; }
            }
        }


        public class Handler : IRequestHandler<RequestApprovalReceiverCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RequestApprovalReceiverCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var allUserList = await _context.UserRoles.ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var requestTransactionExist = await _context.RequestTransactions
                    .FirstOrDefaultAsync(x => x.Id == command.RequestTransactionId, cancellationToken);

                if (requestTransactionExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                foreach (var ticketConcern in command.Concerns)
                {

                    var userExist = await _context.TicketConcerns
                        .FirstOrDefaultAsync(x => x.RequestTransactionId == command.RequestTransactionId
                    && x.UserId == ticketConcern.IssueHandler && x.IsApprove != true);

                    if (userExist == null)
                    {
                        return Result.Failure(TicketRequestError.UserNotExist());
                    }

                    var ticketConcernExist = await _context.TicketConcerns.Include(x => x.RequestorByUser)
                        .Where(x => x.RequestTransactionId == command.RequestTransactionId && x.UserId == userExist.UserId)
                        .ToListAsync();

                    var businessUnitList = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == ticketConcernExist.First().RequestorByUser.BusinessUnitId);
                    var receiverList = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                    if (receiverList == null)
                    {
                        return Result.Failure(TicketRequestError.UnAuthorizedReceiver());
                    }

                    if (receiverList.UserId == command.UserId && receiverPermissionList.Contains(command.Role))
                    {
                        foreach (var concerns in ticketConcernExist)
                        {
                            if (concerns.TargetDate < dateToday)
                            {
                                return Result.Failure(TicketRequestError.DateTimeInvalid());
                            }

                            concerns.IsApprove = true;
                            concerns.ApprovedBy = command.Approved_By;
                            concerns.ApprovedAt = DateTime.Now;
                            concerns.IsReject = false; 
                            //concerns.Remarks = null;
                            concerns.ConcernStatus = TicketingConString.CurrentlyFixing;

                            if (concerns.RequestConcernId != null)
                            {
                                var requestConcernList = await _context.RequestConcerns.Where(x => x.Id == concerns.RequestConcernId).ToListAsync();
                                foreach (var request in requestConcernList)
                                {
                                    request.ConcernStatus = TicketingConString.CurrentlyFixing;
                                }
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
