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
            public Guid ? UserId { get; set; }

            public List<RemoveAttachment> RemoveAttachments { get; set; }
            public class RemoveAttachment
            {
                public int? TicketAttachmentId { get; set; }
            }
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
                foreach(var attachment in command.RemoveAttachments)
                {
                    var attachmentExist = await _context.TicketAttachments
                        .FirstOrDefaultAsync(x => x.Id == attachment.TicketAttachmentId,cancellationToken);

                    if(attachmentExist.AddedBy != command.UserId)
                    {
                        return Result.Failure(TicketRequestError.NotAutorizeToDelete());
                    }

                    if (attachmentExist == null)
                    {
                        return Result.Failure(TicketRequestError.AttachmentNotExist());
                    }

                    attachmentExist.IsActive = false;
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
