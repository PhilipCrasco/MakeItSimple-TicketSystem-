using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class DownloadImageTicketing
    {
        public  class DownloadImageTicketingCommand : IRequest<Result>
        {  
            public int TicketAttachmentId { get; set; }
        }

        public class Handler : IRequestHandler<DownloadImageTicketingCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly ContentType _contentType;

            public Handler(MisDbContext context, ContentType contentType)
            {
                _context = context;
                _contentType = contentType;
            }

            public async Task<Result> Handle(DownloadImageTicketingCommand request, CancellationToken cancellationToken)
            {

                var ticketAttachment = await _context.TicketAttachments
                    .FirstOrDefaultAsync(x => x.Id == request.TicketAttachmentId, cancellationToken);

                if (ticketAttachment == null)
                {
                    return Result<FileStreamResult>.Failure(TicketRequestError.AttachmentNotFound());
                }

                var filePath = ticketAttachment.Attachment;

                if (!System.IO.File.Exists(filePath))
                {
                    return Result<FileStreamResult>.Failure(TicketRequestError.FileNotFound());
                }

                var fileName = Path.GetFileName(filePath);
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = _contentType.GetContentType(fileName);

                var fileResult = new FileStreamResult(new FileStream(filePath, FileMode.Open, FileAccess.Read), contentType)
                {
                    FileDownloadName = fileName
                };

                return Result<FileStreamResult>.Success(fileResult);
            }
        }
    }
}
