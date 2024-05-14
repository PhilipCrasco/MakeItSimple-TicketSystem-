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
            public int ? RequestGeneratorId { get; set; }

            public Guid ? UserId { get; set; }
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public List<RequestComment> RequestComments { get; set; }
            public List<CommentAttachment> CommentAttachments { get; set; }

            public class RequestComment
            {
                public int ?  TicketCommentId { get; set; }
                public string Comment {  get; set; }

            }

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

            public Handler(MisDbContext context, IOptions<CloudinaryOption> options)
            {
                _context = context;
                var account = new Account(
                    options.Value.Cloudname,
                    options.Value.ApiKey,
                    options.Value.ApiSecret
                    );
                _cloudinary = new Cloudinary(account);
            }

            public async Task<Result> Handle(AddTicketCommentCommand command, CancellationToken cancellationToken)
            {
                var prohibitedList = new List<string>();

                var requestGeneratorExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId);
                if (requestGeneratorExist is null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                var uploadTasks = new List<Task>();

                if (command.RequestComments.Count(x => x.Comment != null) > 0)
                {
                    foreach (var comment in command.RequestComments)
                    {

                        var commentExist = await _context.TicketComments
                            .Where(x => x.Id == comment.TicketCommentId).FirstOrDefaultAsync(cancellationToken);

                        var contains = ProhibitedInumerable.Prohibited.FirstOrDefault(word => comment.Comment.ToLower().Contains(word));

                        if (commentExist != null)
                        {
                            if (commentExist.AddedBy != command.UserId)
                            {
                                return Result.Failure(TicketRequestError.NotAutorizeToEdit());
                            }

                            bool IsChange = false;

                            if (commentExist.Comment != comment.Comment)
                            {
                                commentExist.Comment = comment.Comment;
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
                                RequestGeneratorId = command.RequestGeneratorId,
                                Comment = comment.Comment,
                                AddedBy = command.Added_By,
                                IsClicked = false
                            };


                            if (contains != null)
                            {
                                return Result.Failure(TicketRequestError.ProhibitedWord(contains));
                            }

                            await _context.TicketComments.AddAsync(addComment);

                        }
                    }
                }

                else if(command.CommentAttachments.Count(x => x.Attachment != null) > 0)
                {

                    foreach (var attachments in command.CommentAttachments.Where(attachments => attachments.Attachment.Length > 0))
                    {

                        var ticketAttachmentList = await _context.TicketComments.Where(x => x.RequestGeneratorId == requestGeneratorExist.Id).ToListAsync();

                        var ticketAttachment = ticketAttachmentList.FirstOrDefault(x => x.Id == attachments.TicketCommentId);

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
                                PublicId = $"MakeITSimple/Ticketing/Ticket Comment/{requestGeneratorExist.Id}/{attachments.Attachment.FileName}"
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
                                    RequestGeneratorId = command.RequestGeneratorId,
                                    Attachment = attachmentResult.SecureUrl.ToString(),
                                    FileName = attachments.Attachment.FileName,
                                    FileSize = attachments.Attachment.Length,
                                    AddedBy = command.Added_By,
                                };

                                await _context.TicketComments.AddAsync(addAttachment, cancellationToken);

                            }

                        }, cancellationToken));

                    }
                    
                    await Task.WhenAll(uploadTasks);
                }
                else
                {
                    return Result.Failure(TicketRequestError.NoComment());
                }

                await _context.SaveChangesAsync();
                return Result.Success();
            }
        }
    }
}
