using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.CancelTicket
{
    public partial class CancelRequestConcern
    {
        public class Handler : IRequestHandler<CancelRequestConcernCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelRequestConcernCommand command, CancellationToken cancellationToken)
            {

                var requestTransactionExist = await _context.RequestConcerns
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == command.RequestConcernId,cancellationToken);

                if (requestTransactionExist is null)
                    return Result.Failure(TicketRequestError.RequestConcernIdNotExist());
                
                requestTransactionExist.IsActive = false;

                var ticketConcernExist = await _context.TicketConcerns
                .FirstOrDefaultAsync(x => x.Id == requestTransactionExist.Id,cancellationToken);

                ticketConcernExist.IsActive = false;

                var requestAttachment = await _context.TicketAttachments
                .Where(x => x.TicketConcernId == ticketConcernExist.Id)
                .ToListAsync();

                foreach (var cancelAttachment in requestAttachment)
                {
                    cancelAttachment.IsActive = false;
                }


                await TicketNotification(ticketConcernExist, requestTransactionExist, command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private async Task<TicketTransactionNotification> TicketNotification(TicketConcern ticketConcern, RequestConcern requestConcern, CancelRequestConcernCommand command, CancellationToken cancellationToken)
            {
                var userReceiver = await _context.Receivers
                    .FirstOrDefaultAsync(x => x.BusinessUnitId == requestConcern.User.BusinessUnitId);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.Id} has been canceled",
                    AddedBy = requestConcern.UserId.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = userReceiver.UserId.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.ForApproval,
                    PathId = ticketConcern.Id,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                return addNewTicketTransactionNotification;

            }


        }
    }
}
