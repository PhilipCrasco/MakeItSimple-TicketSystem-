using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Common.Cloudinary;
using Microsoft.Extensions.Options;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddTicketComment 
    {
        public class AddTicketCommentCommand : IRequest<Result>
        {
            public int ? TicketConcernId { get; set; }
            public Guid ? UserId { get; set; }
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public int? TicketCommentId { get; set; }
            public string Comment { get; set; }
            public List<CommentAttachment> CommentAttachments { get; set; }

            public class CommentAttachment
            {
                public int ? TicketCommentId {  get; set; }
                public IFormFile Attachment { get; set; }
            }

        }

        public class Handler : IRequestHandler<AddTicketCommentCommand, Result>
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

            public async Task<Result> Handle(AddTicketCommentCommand command, CancellationToken cancellationToken)
            {
                var prohibitedList = new List<string>();

                var uploadTasks = new List<Task>();

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId);

                if (ticketConcernExist is null)
                {
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());
                }

                if(command.Comment is not null)
                {

                    var commentExist = await _context.TicketComments
                        .Where(x => x.Id == command.TicketCommentId)
                        .FirstOrDefaultAsync(cancellationToken);

                    var contains = ProhibitedInumerable.Prohibited
                        .FirstOrDefault(word => command.Comment.ToLower().Contains(word));

                    if (commentExist != null)
                    {
                        if (commentExist.AddedBy != command.UserId)
                        {
                            return Result.Failure(TicketRequestError.NotAutorizeToEdit());
                        }

                        bool IsChange = false;

                        if (commentExist.Comment != command.Comment)
                        {
                            commentExist.Comment = command.Comment;
                        }

                        if (contains != null)
                        {
                            return Result.Failure(TicketRequestError.ProhibitedWord(contains));
                        }

                        if (IsChange)
                        {
                            commentExist.ModifiedBy = command.Modified_By;
                        }

                    }
                    else
                    {

                        var addComment = new TicketComment
                        {
                            TicketConcernId = command.TicketConcernId,
                            Comment = command.Comment,
                            AddedBy = command.Added_By,
                            IsClicked = false
                        };

                        if (contains != null)
                        {
                            return Result.Failure(TicketRequestError.ProhibitedWord(contains));
                        }

                        await _context.TicketComments.AddAsync(addComment);
                        await _context.SaveChangesAsync(cancellationToken);

                        commentExist = addComment;
                    }

                }
                else if(command.CommentAttachments.Any())
                {
                    foreach (var attachments in command.CommentAttachments.Where(attachments => attachments.Attachment.Length > 0))
                    {

                        var ticketAttachment = await _context.TicketComments
                            .FirstOrDefaultAsync(x => x.Id == attachments.TicketCommentId, cancellationToken);

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
                                PublicId = $"MakeITSimple/Ticketing/Ticket Comment/{userDetails.Fullname}/{attachments.Attachment.FileName}",
                            };

                            var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);

                            if (ticketAttachment != null)
                            {
                                var hasChanged = false;

                                if (ticketAttachment.Attachment != attachmentResult.SecureUrl.ToString())
                                {
                                    ticketAttachment.Attachment = attachmentResult.SecureUrl.ToString();
                                    hasChanged = true;
                                }

                                if (hasChanged)
                                {
                                    ticketAttachment.ModifiedBy = command.Modified_By;
                                    ticketAttachment.UpdatedAt = DateTime.Now;
                                    ticketAttachment.FileName = attachments.Attachment.FileName;
                                    ticketAttachment.FileSize = attachments.Attachment.Length;

                                }

                            }
                            else
                            {
                                var addAttachment = new TicketComment
                                {
                                    TicketConcernId = command.TicketConcernId,
                                    Attachment = attachmentResult.SecureUrl.ToString(),
                                    FileName = attachments.Attachment.FileName,
                                    FileSize = attachments.Attachment.Length,
                                    AddedBy = command.Added_By,
                                };

                                await _context.TicketComments.AddAsync(addAttachment, cancellationToken);

                            }

                        }, cancellationToken));

                    }
                }
                else
                {
                    return Result.Failure(TicketRequestError.NoComment());
                }

                await Task.WhenAll(uploadTasks);

                await _context.SaveChangesAsync();
                return Result.Success();
            }
        }
    }
}
