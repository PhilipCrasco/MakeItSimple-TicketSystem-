using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosingTicket
{
    public class CancelClosingTicket
    {
        public class CancelClosingTicketCommand : IRequest<Result>
        {
            public List<CancelClosingGenerator> CancelClosingGenerators { get; set; }
            public class CancelClosingGenerator
            {
                public int ClosingGeneratorId { get; set; }

                public List<CancelClosingConcern> CancelClosingConcerns { get; set; }
                public List<CancelClosingAttachment> CancelClosingAttachments { get; set; }

                public class CancelClosingConcern
                {
                    public int ? TicketConcernId { get; set; }
                }

                public class CancelClosingAttachment

                {
                    public int? ClosingTAttachmentId { get; set; }
                }


            }

        }

        public class Handler : IRequestHandler<CancelClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelClosingTicketCommand command, CancellationToken cancellationToken)
            {
                
                foreach(var ticketConcern in command.CancelClosingGenerators)
                {

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
