using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class UpsertTransferTicketConcern
    {
        public class UpsertTransferTicketConcernCommand : IRequest<Result>
        {
            public Guid ? Transfer_By { get; set; }
            public Guid ? Added_By { get; set; }
            public List<AddTransferTicket> AddTransferTickets { get; set; }

            public class AddTransferTicket
            {
                public int ? TicketConcernId { get; set; }
                public int ? TransferTicketConcernId {  get; set; }
                public int SubUnitId { get; set; }
                public int ChannelId { get; set; }
                public Guid? UserId { get; set; }
            }
        }

        public class Handler : IRequestHandler<UpsertTransferTicketConcernCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertTransferTicketConcernCommand command, CancellationToken cancellationToken)
            {
                foreach(var transferConcern in command.AddTransferTickets)
                {

                    var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == transferConcern.TicketConcernId, cancellationToken);
                    if(ticketConcern != null) 
                    {

                        var addTransferTicket = new TransferTicketConcern
                        {

                        };

                        await _context.TransferTicketConcerns.AddAsync(addTransferTicket, cancellationToken);




                    }
                    


                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }

    }
}
