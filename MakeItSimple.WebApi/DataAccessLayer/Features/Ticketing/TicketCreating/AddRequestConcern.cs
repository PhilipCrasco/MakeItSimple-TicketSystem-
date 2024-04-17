using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddRequestConcern
    {

        public class AddRequestConcernCommand : IRequest<Result>
        {
            
            public int ? RequestGeneratorId { get; set; }
            public Guid ? Added_By { get; set; } 
            public Guid ? Modified_By { get; set; }
            public Guid ? UserId { get; set; }
            public int? RequestConcernId { get; set; }
            public string Concern { get; set; }

            public List<RequestAttachmentsFile> RequestAttachmentsFiles {  get; set; }

            public class RequestAttachmentsFile
            {
                public int ? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }


            }

        }

        public class Handler : IRequestHandler<AddRequestConcernCommand, Result>
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

            public async Task<Result> Handle(AddRequestConcernCommand command, CancellationToken cancellationToken)
            {

                var requestGeneratorList = new List<RequestGenerator>();
                var requestConcernList = new List<RequestConcern>();


                var requestGeneratorexist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId, cancellationToken);
                if (requestGeneratorexist == null)
                {
                    var requestGeneratorId = new RequestGenerator { IsActive = true };
                    await _context.RequestGenerators.AddAsync(requestGeneratorId);
                    requestGeneratorexist = requestGeneratorId;
                }

                var userId = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId);
                if (userId == null)
                {
                    return Result.Failure(UserError.UserNotExist());
                }

                await _context.SaveChangesAsync(cancellationToken);
                requestGeneratorList.Add(requestGeneratorexist);

                var requestConcernIdExist = await _context.RequestConcerns
                    .Include(x => x.User).ThenInclude(x => x.Department)
                    .FirstOrDefaultAsync(x => x.Id == command.RequestConcernId, cancellationToken);
                if (requestConcernIdExist != null)
                {

                    var ticketConcernExist = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.RequestConcernId == requestConcernIdExist.Id, cancellationToken);

                    bool isChange = false;

                    if (requestConcernIdExist.Concern != command.Concern)
                    {
                        requestConcernIdExist.Concern = command.Concern;
                        isChange = true;
                    }

                    if (isChange)
                    {
                        requestConcernIdExist.ModifiedBy = command.Modified_By;
                        ticketConcernExist.ConcernDetails = requestConcernIdExist.Concern;
                    }

                    requestConcernList.Add(requestConcernIdExist);

                }
                else
                {

                    var addRequestConcern = new RequestConcern
                    {
                        RequestGeneratorId = requestGeneratorexist.Id,
                        UserId = userId.Id,
                        Concern = command.Concern,
                        AddedBy = command.Added_By,
                        ConcernStatus = TicketingConString.ForApprovalTicket,
                        IsDone = false

                    };

                    await _context.RequestConcerns.AddAsync(addRequestConcern);
                    await _context.SaveChangesAsync(cancellationToken);


                    var addTicketConcern = new TicketConcern
                    {
                        RequestGeneratorId = requestGeneratorexist.Id,
                        RequestConcernId = addRequestConcern.Id,
                        RequestorBy = command.UserId,
                        ConcernDetails = addRequestConcern.Concern,
                        IsApprove = false,
                        AddedBy = command.Added_By,
                        TicketType = TicketingConString.Concern,
                        ConcernStatus = addRequestConcern.ConcernStatus

                    };

                    await _context.TicketConcerns.AddAsync(addTicketConcern);
                    requestConcernList.Add(addRequestConcern);
                }


                var uploadTasks = new List<Task>();

                if (command.RequestAttachmentsFiles.Count(x => x.Attachment != null) > 0 )
                {
                    foreach (var attachments in command.RequestAttachmentsFiles.Where(attachments => attachments.Attachment.Length > 0))
                    {

                        //var ticketAttachmentList = await _context.TicketAttachments.Include(x => x.RequestGenerator)
                        //.Where(x => x.RequestGeneratorId == requestGeneratorList.First().Id).ToListAsync();

                        var ticketAttachment = await _context.TicketAttachments.FirstOrDefaultAsync(x => x.Id == attachments.TicketAttachmentId, cancellationToken);

                        if (attachments.Attachment == null || attachments.Attachment.Length == 0)
                        {
                            return Result.Failure(TicketRequestError.AttachmentNotNull());
                        }

                        if (attachments.Attachment.Length > 10 * 1024 * 1024)
                        {
                            return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                        }

                        var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx" , ".pdf",".xlsx" };
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
                                PublicId = $"MakeITSimple/Ticketing/Request/{requestGeneratorList.First().Id}/{attachments.Attachment.FileName}"
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
                                    ticketAttachment.FileName = attachments.Attachment.FileName;
                                    ticketAttachment.FileSize = attachments.Attachment.Length;
                                    ticketAttachment.UpdatedAt = DateTime.Now;
                                }

                            }
                            else
                            {
                                var addAttachment = new TicketAttachment
                                {
                                    RequestGeneratorId = requestGeneratorList.First().Id,
                                    Attachment = attachmentResult.SecureUrl.ToString(),
                                    FileName = attachments.Attachment.FileName,
                                    FileSize = attachments.Attachment.Length,
                                    AddedBy = command.Added_By,
                                };

                                await _context.AddAsync(addAttachment, cancellationToken);

                            }
                     
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
