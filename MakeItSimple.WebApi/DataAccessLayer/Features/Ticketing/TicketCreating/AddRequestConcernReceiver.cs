using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddRequestConcernReceiver
    {
        public class AddRequestConcernReceiverCommand : IRequest<Result>
        {
            public int? TicketConcernId { get; set; }
            public int ? ChannelId { get; set; }
            public Guid ? Requestor_By { get; set; }
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public Guid? UserId { get; set; }
            public string Concern_Details { get; set; }
            public int CategoryId { get; set; }
            public int SubCategoryId { get; set; }
            public DateTime Start_Date { get; set; }
            public DateTime Target_Date { get; set; }
            public string Role { get; set; }
            public string Remarks { get; set; } 
            public string Modules { get; set; }

            public List<ConcernAttachment> ConcernAttachments {  get; set; }

            public class ConcernAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }

        }

        public class Handler : IRequestHandler<AddRequestConcernReceiverCommand, Result>
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

            public async Task<Result> Handle(AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var allUserList = await _context.UserRoles
                    .ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();

                var requestorPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Requestor)).Select(x => x.UserRoleName).ToList();

                var channelExist = await _context.Channels
                    .FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);

                if(channelExist == null)
                {
                    return Result.Failure(TicketRequestError.ChannelNotExist());
                }

                switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.UserNotExist());
                }



                if (dateToday > command.Target_Date)
                {
                    return Result.Failure(TicketRequestError.DateTimeInvalid());
                }


                var upsertConcern = await _context.TicketConcerns
                .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId);

                if (upsertConcern != null)
                {

                    bool hasChanged = false;

                    if (upsertConcern.ChannelId != command.ChannelId)
                    {
                        upsertConcern.ChannelId = command.ChannelId;
                        hasChanged = true;
                    }

                    if (upsertConcern.UserId != command.UserId)
                    {
                        upsertConcern.UserId = command.UserId;
                        hasChanged = true;
                    }

                    if (upsertConcern.TargetDate != command.Target_Date)
                    {
                        upsertConcern.TargetDate = command.Target_Date;
                        hasChanged = true;
                    }


                    if (upsertConcern.RequestConcernId != null)
                    {
                        var requestConcern = await _context.RequestConcerns
                            .FirstOrDefaultAsync(x => x.Id == upsertConcern.RequestConcernId, cancellationToken);

                        requestConcern.Remarks = null;
                    }

                    if (hasChanged)
                    {
                        upsertConcern.ModifiedBy = command.Modified_By;
                        upsertConcern.UpdatedAt = DateTime.Now;
                        upsertConcern.IsAssigned = true;
                        
                    }

                    var addNewTicketTransactionNotification = new TicketTransactionNotification
                    {

                        Message = $"Ticket number {upsertConcern.Id} has been assigned",
                        AddedBy = userDetails.Id,
                        Created_At = DateTime.Now,
                        ReceiveBy = command.UserId.Value,
                        Modules = PathConString.IssueHandlerConcerns,
                        Modules_Parameter = PathConString.OpenTicket,
                        PathId = upsertConcern.Id,
                        
                    };

                    await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                    if(upsertConcern.IsApprove != true)
                    {
                        var addNewTicketTransactionOngoing = new TicketTransactionNotification
                        {

                            Message = $"Ticket number {upsertConcern.RequestConcernId} is now ongoing",
                            AddedBy = userDetails.Id,
                            Created_At = DateTime.Now,
                            ReceiveBy = command.UserId.Value,
                            Modules = PathConString.ConcernTickets,
                            Modules_Parameter = PathConString.Ongoing,
                            PathId = upsertConcern.RequestConcernId.Value,

                        };

                        await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionOngoing);

                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.ConcernAttachments.Count(x => x.Attachment != null) > 0)
                {
                    foreach (var attachments in command.ConcernAttachments.Where(a => a.Attachment.Length > 0))
                    {

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

                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(TicketingConString.AttachmentPath, fileName);

                        var ticketAttachment = await _context.TicketAttachments
                            .FirstOrDefaultAsync(x => x.Id == attachments.TicketAttachmentId, cancellationToken);

                        if (ticketAttachment != null)
                        {
                            ticketAttachment.Attachment = filePath;
                            ticketAttachment.FileName = attachments.Attachment.FileName;
                            ticketAttachment.FileSize = attachments.Attachment.Length;
                            ticketAttachment.UpdatedAt = DateTime.Now;

                        }
                        else
                        {
                            var addAttachment = new TicketAttachment
                            {
                                TicketConcernId = upsertConcern.Id,
                                Attachment = filePath,
                                FileName = attachments.Attachment.FileName,
                                FileSize = attachments.Attachment.Length,
                                AddedBy = command.Added_By,
                            };

                            await _context.TicketAttachments.AddAsync(addAttachment);

                        }

                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await attachments.Attachment.CopyToAsync(stream);
                        }
                    }
                }

                await _context.SaveChangesAsync(cancellationToken); 
                return Result.Success();
            }
        }
    }
}
