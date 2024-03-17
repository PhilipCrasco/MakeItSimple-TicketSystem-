using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class ReturnRequestTicket
    {
        public class ReturnRequestTicketCommand : IRequest<Result>
        {

            public string Remarks { get; set; }
            public List<ReturnTicketRequestById> ReturnTicketRequestByIds {  get; set; }

            public class ReturnTicketRequestById
            {
                public int? RequestGeneratorId { get; set; }
            }

            
        }

        public class Handler : IRequestHandler<ReturnRequestTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }


            public async Task<Result> Handle(ReturnRequestTicketCommand command, CancellationToken cancellationToken)
            {
                foreach (var ticketConcern in command.ReturnTicketRequestByIds)
                {
                    var requestGeneratorExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == ticketConcern.RequestGeneratorId, cancellationToken);
                    if (requestGeneratorExist == null)
                    {
                        return Result.Failure(TicketRequestError.TicketIdNotExist());
                    }

                    var ticketConcernExist = await _context.TicketConcerns.Where(x => x.RequestGeneratorId == ticketConcern.RequestGeneratorId).ToListAsync();

                    foreach (var concerns in ticketConcernExist)
                    {
                        concerns.IsReject = false;
                        concerns.IsApprove = false;
                        concerns.Remarks = command.Remarks;
                    }


                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
