using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class ApproveRequestTicket
    {

        public class ApproveRequestTicketCommand : IRequest<Result>
        {
            public Guid ? Approved_By { get; set; }
            public Guid ? UserId { get; set; }

            public string Role { get; set; }

            public List<Concerns> Concern { get; set; }

            public class Concerns
            {
                public int RequestGeneratorId { get; set; }
                public Guid ? IssueHandler { get; set; }
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

                    var requestTicketId = await _context.ApproverTicketings
                    .Where(x => x.RequestGeneratorId == requestGeneratorExist.Id && x.IsApprove == null).ToListAsync();

                    var userExist = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.RequestGeneratorId == ticketConcern.RequestGeneratorId
                    && x.UserId == ticketConcern.IssueHandler && x.IsApprove != true);

                    if (userExist == null)
                    {
                        return Result.Failure(TicketRequestError.UserNotExist());
                    }

                    var ticketConcernExist = await _context.TicketConcerns.Include(x => x.RequestorByUser)
                        .Where(x => x.RequestGeneratorId == ticketConcern.RequestGeneratorId && x.UserId == userExist.UserId).ToListAsync();

                    var ticketConcernHandlerExist = await _context.TicketConcerns.Include(x => x.RequestorByUser).Where(x => x.RequestGeneratorId == ticketConcern.RequestGeneratorId 
                    && x.TicketApprover != null && x.UserId == userExist.UserId).ToListAsync();

                    var requestTicketConcernId = await _context.ApproverTicketings
                     .Where(x => x.RequestGeneratorId == requestGeneratorExist.Id && x.IsApprove == null && x.IssueHandler == userExist.UserId).ToListAsync();

                    var selectRequestTicketId = requestTicketConcernId.FirstOrDefault(x => x.ApproverLevel == requestTicketConcernId.Min(x => x.ApproverLevel));

                    if(selectRequestTicketId != null)
                    {
                        selectRequestTicketId.IsApprove = true;

                        if (ticketConcernHandlerExist.First().TicketApprover != command.UserId)
                        {
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                        }

                        var userApprovalId = await _context.ApproverTicketings.Include(x => x.User).Where(x => x.RequestGeneratorId == selectRequestTicketId.RequestGeneratorId).ToListAsync();

                        foreach (var concernTicket in ticketConcernHandlerExist)
                        {

                            var validateUserApprover = userApprovalId.FirstOrDefault(x => x.ApproverLevel == selectRequestTicketId.ApproverLevel + 1);

                            if (validateUserApprover != null)
                            {
                                concernTicket.TicketApprover = validateUserApprover.UserId;
                                concernTicket.ConcernStatus = $"{TicketingConString.RequestApproval} {validateUserApprover.User.Username}";
                            }
                            else
                            {
                                concernTicket.TicketApprover = null;
                                concernTicket.ConcernStatus = $"{TicketingConString.RequestApproval} Receiver";
                            }

                        }

                    }
                    else
                    {
                        var businessUnitList = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == ticketConcernExist.First().RequestorByUser.BusinessUnitId);
                        var receiverList = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                        if (receiverList == null)
                        {
                            return Result.Failure(TicketRequestError.UnAuthorizedReceiver());
                        }

                        if (receiverList.UserId == command.UserId && command.Role == TicketingConString.Receiver)
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
                                concerns.Remarks = null;
                                concerns.ConcernStatus = TicketingConString.CurrentlyFixing;
                                ticketApproveList.Add(concerns);

                                if(concerns.RequestConcernId != null)
                                {
                                    var requestConcernList = await _context.RequestConcerns.Where(x => x.Id == concerns.RequestConcernId).ToListAsync();
                                    foreach(var request in requestConcernList)
                                    {
                                         request.ConcernStatus = TicketingConString.CurrentlyFixing;
                                    }
                                }
                            }
                        }
                        else
                        {
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                        }
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

        }
    }


}
