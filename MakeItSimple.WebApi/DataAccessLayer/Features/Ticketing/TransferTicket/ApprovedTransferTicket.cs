using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class ApprovedTransferTicket
    {
       public class ApprovedTransferTicketCommand : IRequest<Result>
        {
            public Guid ? Transfer_By {  get; set; }
            public ICollection<ApproveTransferTicket> ApproveTransferTickets { get; set; }
            public class ApproveTransferTicket
            {
                public int TicketConcernId { get; set; }
            }

        }

        public class Handler : IRequestHandler<ApprovedTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                
                foreach(var transferTicket in command.ApproveTransferTickets)
                {
                    var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == transferTicket.TicketConcernId ,cancellationToken);

                    if (ticketConcern == null)
                    {
                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }

                    ticketConcern.TransferAt = DateTime.Now;
                    ticketConcern.TransferBy = command.Transfer_By;
                    ticketConcern.IsTransfer = true;
                    ticketConcern.IsApprove = null;
                   
                    var transferConcern = await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == transferTicket.TicketConcernId 
                    && x.IsTransfer != true, cancellationToken);

                    transferConcern.TransferAt = DateTime.Now;
                    transferConcern.TransferBy = command.Transfer_By;
                    transferConcern.IsTransfer = true;
                    ticketConcern.SubUnitId = transferConcern.SubUnitId;
                    ticketConcern.SubUnitId = transferConcern.SubUnitId;
                    ticketConcern.ChannelId = transferConcern.ChannelId;
                    ticketConcern.UserId = transferConcern.UserId;  
                  
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
