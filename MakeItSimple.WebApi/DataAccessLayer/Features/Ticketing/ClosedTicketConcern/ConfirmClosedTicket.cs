using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class ConfirmClosedTicket
    {
        public class ConfirmClosedTicketCommand : IRequest<Result>
        {
            public int? RequestConcernId { get; set; }
        }

        public class Hanler : IRequestHandler<ConfirmClosedTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Hanler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ConfirmClosedTicketCommand request, CancellationToken cancellationToken)
            {



                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
