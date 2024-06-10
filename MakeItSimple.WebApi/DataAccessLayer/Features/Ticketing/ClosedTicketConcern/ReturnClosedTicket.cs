using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class ReturnClosedTicket
    {

        public class ReturnClosedTicketCommand : IRequest<Result>
        {
            public int? RequestConcernId { get; set; }
            public string Remarks { get; set; }

            public Guid ? Added_By { get; set; }

            public List<ReturnTicketAttachment> ReturnTicketAttachments { get; set; }
            public class ReturnTicketAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }
        }

        public class Handler : IRequestHandler<ReturnClosedTicketCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly Cloudinary _cloudinary;
            private readonly TransformUrl _url;

            public Handler(MisDbContext context, IOptions<CloudinaryOption> options, TransformUrl url)
            {
                _context = context;
                var account = new Account(
                    options.Value.Cloudname,
                    options.Value.ApiKey,
                    options.Value.ApiSecret
                    );
                _cloudinary = new Cloudinary(account);
                _url = url;
            }

            public async Task<Result> Handle(ReturnClosedTicketCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await _context.Users
                   .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var requestConcernExist = await _context.RequestConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.RequestConcernId, cancellationToken);

                if (requestConcernExist is null)
                {
                    return Result.Failure(TicketRequestError.RequestConcernIdNotExist());
                }

                var ticketConcernExist = await _context.TicketConcerns
                     .FirstOrDefaultAsync(x => x.RequestConcernId == requestConcernExist.Id, cancellationToken);

                ticketConcernExist.IsClosedApprove = null;
                ticketConcernExist.Closed_At = null;
                ticketConcernExist.ClosedApproveBy = null;


                var uploadTasks = new List<Task>();

                if (command.ReturnTicketAttachments.Count(x => x.Attachment != null) > 0)
                {

                    foreach (var attachments in command.ReturnTicketAttachments.Where(attachments => attachments.Attachment.Length > 0))
                    {

                        var ticketAttachment = await _context.TicketAttachments
                         .FirstOrDefaultAsync(x => x.Id == attachments.TicketAttachmentId, cancellationToken);

                        if (attachments.Attachment == null || attachments.Attachment.Length == 0)
                        {
                            return Result.Failure(TicketRequestError.AttachmentNotNull());
                        }

                        if (attachments.Attachment.Length > 10 * 1024 * 1024)
                        {
                            return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                        }

                        var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                        var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                        if (extension == null || !allowedFileTypes.Contains(extension))
                        {
                            return Result.Failure(TicketRequestError.InvalidAttachmentType());
                        }

                        uploadTasks.Add(Task.Run(async () =>
                        {
                            await using var stream = attachments.Attachment.OpenReadStream();

                            var attachmentsParams = new RawUploadParams
                            {
                                File = new FileDescription(attachments.Attachment.FileName, stream),
                                PublicId = $"MakeITSimple/Ticketing/Request/{userDetails.Fullname}/{attachments.Attachment.FileName}"
                            };

                            var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);
                            string attachmentUrl = attachmentResult.SecureUrl.ToString();
                            string transformedUrl = _url.TransformUrlForViewOnly(attachmentUrl, attachments.Attachment.FileName);

                            var addAttachment = new TicketAttachment
                            {
                                TicketConcernId = ticketConcernExist.Id,
                                Attachment = attachmentResult.SecureUrl.ToString(),
                                FileName = attachments.Attachment.FileName,
                                FileSize = attachments.Attachment.Length,
                                AddedBy = command.Added_By,

                            };

                            await _context.TicketAttachments.AddAsync(addAttachment);

                        }, cancellationToken));

                    }
                }

                await Task.WhenAll(uploadTasks);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
