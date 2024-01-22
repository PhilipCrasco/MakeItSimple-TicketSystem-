using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class RejectTransferTicket
    {

        public class RejectTransferTicketCommand : IRequest<Result>
        {
            public ICollection<RejectTransferTicket> RejectTransferTickets { get; set; }
            public class RejectTransferTicket
            {
                public int TicketConcernId { get; set; }
            }
        }

        public class Handler : IRequestHandler<RejectTransferTicketCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectTransferTicketCommand command, CancellationToken cancellationToken)
            {
                foreach(var transferTicket in command.RejectTransferTickets)
                {
                    var transferConcern= await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == transferTicket.TicketConcernId, cancellationToken);

                    if (transferConcern == null)
                    {
                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }

                    transferConcern.IsActive = false;
                }

                return Result.Success("Reject Success");
            }
        }
    }
}
