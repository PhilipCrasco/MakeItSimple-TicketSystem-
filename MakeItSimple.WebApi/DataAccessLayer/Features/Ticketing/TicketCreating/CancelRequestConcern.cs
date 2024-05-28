using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class CancelRequestConcern
    {
        public class CancelRequestConcernCommand : IRequest<Result>
        {

            public int? RequestConcernId { get; set; }
            public List<RequestAttachment> RequestAttachments { get; set; }
            public class RequestAttachment
            {
                public int? TicketAttachmentId { get; set; }
            }

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
        
                await _context.SaveChangesAsync(cancellationToken);  
                return Result.Success();
            }
        }
    }
}
