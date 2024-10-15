using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApprovalTicket
{
    public partial class RequestApprovalReceiver
    {


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

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken);

                var allUserList = await _context
                    .UserRoles
                    .ToListAsync();

                var receiverPermissionList = allUserList
                    .Where(x => x.Permissions
                .Contains(TicketingConString.Receiver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var ticketConcernExist = await _context.TicketConcerns
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId
                    && x.IsApprove != true, cancellationToken);

                if (ticketConcernExist is null)
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());

                var businessUnitList = await _context.BusinessUnits
                .FirstOrDefaultAsync(x => x.Id == ticketConcernExist.RequestorByUser.BusinessUnitId);

                var receiverList = await _context.Receivers
                .FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);

                var validate = await ValidationHandler(ticketConcernExist, receiverList,command ,cancellationToken);
                if (validate is not null)
                    return validate;
                

                if (receiverList.UserId == command.UserId && receiverPermissionList.Contains(command.Role))
                {
                    await UpdateTicket(ticketConcernExist, command, cancellationToken);

                    await RequestTicketHistory(userDetails, ticketConcernExist,command,cancellationToken);

                }
                else
                {
                    return Result.Failure(TicketRequestError.NotAutorize());
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }




            private async Task<Result?> ValidationHandler(TicketConcern ticketConcern , Receiver receiver , RequestApprovalReceiverCommand command, CancellationToken cancellationToken)
            {   

                if (receiver is null)        
                    return Result.Failure(TicketRequestError.NoReceiver());

                return null;
                

            }

            private async Task<TicketConcern> UpdateTicket(TicketConcern ticketConcern, RequestApprovalReceiverCommand command, CancellationToken cancellationToken)
            {
                ticketConcern.IsApprove = true;
                ticketConcern.ApprovedBy = command.Approved_By;
                ticketConcern.ApprovedAt = DateTime.Now;
                ticketConcern.ConcernStatus = TicketingConString.CurrentlyFixing;

                if (ticketConcern.RequestConcernId is not null)
                {
                    var requestConcernList = await _context.RequestConcerns
                    .FirstOrDefaultAsync(x => x.Id == ticketConcern.RequestConcernId);

                    requestConcernList.ConcernStatus = TicketingConString.CurrentlyFixing;
                }

                return ticketConcern;

            }

            private async Task<TicketHistory> RequestTicketHistory(User user,TicketConcern ticketConcern, RequestApprovalReceiverCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcern.Id,
                    TransactedBy = command.UserId,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.ConcernAssign,
                    Status = $"{TicketingConString.RequestAssign} {user.Fullname}"
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                return addTicketHistory;
            }


        }
    }
}
