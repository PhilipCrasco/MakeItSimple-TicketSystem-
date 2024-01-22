using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class CancelTransferTicket
    {
        public class CancelTransferTicketCommand : IRequest<Result>
        {
           public List<CancelTransferTicketConcern> CancelTransferTicketConcerns { get; set; }

            public class CancelTransferTicketConcern
            {
                public int TransferTicketConcernId { get; set; }
            }
        }

        public class Handler : IRequestHandler<CancelTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelTransferTicketCommand command, CancellationToken cancellationToken)
            {


                foreach(var transferTicket in command.CancelTransferTicketConcerns)
                {
                    var transferTicketQuery = await _context.TransferTicketConcerns.Where(x => x.Id == transferTicket.TransferTicketConcernId).ToListAsync();

                    if(transferTicketQuery == null)
                    {
                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }

                    _context.Remove(transferTicketQuery);

                }

                return Result.Success("Cancel Successful");
            }
        }
    }
}
