using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class EditTargetDate
    {

        public class EditTargetDateCommand : IRequest<Result>
        {

            public ICollection<EditDate> EditDates {get; set;}
            public class EditDate
            {
                public int TicketConcernId { get; set; }
                public DateTime Start_Date { get; set; }
                public DateTime Target_Date { get; set; }
            }
        }

        public class Handler : IRequestHandler<EditTargetDateCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {  
                _context = context;
            }

            public async Task<Result> Handle(EditTargetDateCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                foreach(var transferTicket in command.EditDates)
                {
                    if(transferTicket.Start_Date > transferTicket.Target_Date || transferTicket.Target_Date < dateToday)
                    {
                        return Result.Failure(TransferTicketError.DateTimeInvalid());
                    }
                   
                    var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == transferTicket.TicketConcernId, cancellationToken);

                    if (ticketConcern == null)
                    {
                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }

                    ticketConcern.StartDate = transferTicket.Start_Date;
                    ticketConcern.TargetDate = transferTicket.Target_Date;
                    ticketConcern.IsApprove = false;
                    
                    
                }

                return Result.Success("Edit Success");
            }
        }

    }
}
