using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class CancelTicketConcern
    {
        public class CancelTicketConcernCommand : IRequest<Result>
        {
            public int TicketConcernId { get; set; }
            public string Role { get; set; }
            public string Modules { get; set; }

        }

        public class Handler : IRequestHandler<CancelTicketConcernCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelTicketConcernCommand command, CancellationToken cancellationToken)
            {

                var ticketConcernExist = await _context.TicketConcerns
                    .Include(x => x.RequestorByUser)
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);

                if (ticketConcernExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());
                }

                var userRoleList = await _context.UserRoles.ToListAsync();

                var receiverPermission = userRoleList
                .Where(x => x.Permissions.Contains(TicketingConString.Receiver))
                .Select(x => x.UserRoleName);

                if(!receiverPermission.Any(x => x.Contains(command.Role)))
                {
                    return Result.Failure(TicketRequestError.UnAuthorizedReceiver());
                }

                ticketConcernExist.IsActive = false;

                var requestConcernExist = await _context.RequestConcerns
                    .FirstOrDefaultAsync(x => x.Id == ticketConcernExist.RequestConcernId);

                requestConcernExist.IsActive = false;

                var ticketAttachmentList =  await _context.TicketAttachments
                    .Where(x => x.TicketConcernId == ticketConcernExist.Id)
                    .ToListAsync();

                foreach (var ticketAttachment in ticketAttachmentList)
                {
                    ticketAttachment.IsActive = false;
                }

                var userReceiver = await _context.Receivers
                    .FirstOrDefaultAsync(x => x.BusinessUnitId == ticketConcernExist.RequestorByUser.BusinessUnitId);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcernExist.Id} has been canceled",
                    AddedBy = ticketConcernExist.RequestorBy.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = userReceiver.UserId.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.ForApproval,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);


                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
