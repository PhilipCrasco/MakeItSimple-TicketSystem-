using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class RemoveTicketAttachment
    {
        public class RemoveTicketAttachmentCommand : IRequest<Result>
        {
            public int? TicketAttachmentId { get; set; }
        }

        public class Handler : IRequestHandler<RemoveTicketAttachmentCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RemoveTicketAttachmentCommand command, CancellationToken cancellationToken)
            {

                    var attachmentExist = await _context.TicketAttachments
                        .FirstOrDefaultAsync(x => x.Id == command.TicketAttachmentId,cancellationToken);


                    if (attachmentExist == null)
                    {
                        return Result.Failure(TicketRequestError.AttachmentNotExist());
                    }

                    _context.Remove(attachmentExist);
                

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
