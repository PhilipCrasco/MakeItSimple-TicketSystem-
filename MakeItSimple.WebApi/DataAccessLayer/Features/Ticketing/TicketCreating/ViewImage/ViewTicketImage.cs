

using Azure;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ViewImage
{
    public partial class ViewTicketImage
    {

        public class Handler : IRequestHandler<ViewTicketImageCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly ContentType _contentType;

            public Handler(MisDbContext context, ContentType contentType)
            {
                _context = context;
                _contentType = contentType;
            }

            public async Task<Result> Handle(ViewTicketImageCommand request, CancellationToken cancellationToken)
            {

                var ticketAttachment = await _context.TicketAttachments
                    .FirstOrDefaultAsync(x => x.Id == request.TicketAttachmentId, cancellationToken);

                if (ticketAttachment is null)
                    return Result.Failure(TicketRequestError.AttachmentNotFound());

                var filePath = ticketAttachment.Attachment;

                if (!File.Exists(filePath))
                    return Result.Failure(TicketRequestError.FileNotFound());
                

                var fileName = Path.GetFileName(filePath);
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = _contentType.GetContentType(fileName);

                var fileResult = new FileStreamResult(new FileStream(filePath, FileMode.Open, FileAccess.Read), contentType);

                return Result.Success(fileResult);
            }

        }
    }
}
