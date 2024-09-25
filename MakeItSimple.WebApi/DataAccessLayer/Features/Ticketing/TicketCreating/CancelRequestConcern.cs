using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class CancelRequestConcern
    {
        public class CancelRequestConcernCommand : IRequest<Result>
        {

            public int? RequestConcernId { get; set; }
            public string Modules { get; set; }


        }
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
                    .FirstOrDefaultAsync(x => x.Id == command.RequestConcernId);

                if (requestTransactionExist == null)
                {
                    return Result.Failure(TicketRequestError.RequestConcernIdNotExist());
                }

                requestTransactionExist.IsActive = false;

                var ticketConcernExist = await _context.TicketConcerns
                .FirstOrDefaultAsync(x => x.Id == requestTransactionExist.Id);

                ticketConcernExist.IsActive = false;

                var requestAttachment = await _context.TicketAttachments
                .Where(x => x.TicketConcernId == ticketConcernExist.Id)
                .ToListAsync();

                foreach (var cancelAttachment in requestAttachment)
                {
                    cancelAttachment.IsActive = false;
                }

                var userReceiver = await _context.Receivers
                    .FirstOrDefaultAsync(x => x.BusinessUnitId == requestTransactionExist.User.BusinessUnitId);


                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcernExist.Id} has been canceled",
                    AddedBy = requestTransactionExist.UserId.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = userReceiver.UserId.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.ForApproval,
                    PathId = ticketConcernExist.Id,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                await _context.SaveChangesAsync(cancellationToken);  
                return Result.Success();
            }
        }
    }
}
