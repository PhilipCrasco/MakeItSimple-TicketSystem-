using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Xml.Schema;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AssignTicket
{
    public partial class AddRequestConcernReceiver
    {

        public class Handler : IRequestHandler<AddRequestConcernReceiverCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var allUserList = await _context.UserRoles
                    .ToListAsync();

                var receiverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var issueHandlerPermissionList = allUserList
                    .Where(x => x.Permissions
                .Contains(TicketingConString.IssueHandler))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var requestorPermissionList = allUserList
                    .Where(x => x.Permissions
                .Contains(TicketingConString.Requestor))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var validation = await ValidationHandler(command, cancellationToken);
                if(validation is not null) 
                    return validation;
                
                var upsertConcern = await _context.TicketConcerns
                .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId,cancellationToken);

                if (upsertConcern is not null)
                {
                    await AssignTicket(upsertConcern,command,cancellationToken);

                    await TransactionNotification(upsertConcern, userDetails, command, cancellationToken);

                    await RequestorTransactionNotification(upsertConcern, userDetails, command, cancellationToken);

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
                    await AttachmentHandler(upsertConcern,command, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

  
            private async Task<Result?> ValidationHandler(AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;


                switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.UserNotExist());
                }

                if (dateToday > command.Target_Date)
                    return Result.Failure(TicketRequestError.DateTimeInvalid());
                

                return null;
            }

            private async Task<TicketConcern> AssignTicket(TicketConcern ticketConcern , AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {

                bool hasChanged = false;

                if (ticketConcern.UserId != command.UserId)
                {
                    ticketConcern.UserId = command.UserId;
                    hasChanged = true;
                }

                if (ticketConcern.TargetDate != command.Target_Date)
                {
                    ticketConcern.TargetDate = command.Target_Date;
                    hasChanged = true;
                }


                if (ticketConcern.RequestConcernId is not null)
                {
                    var requestConcern = await _context.RequestConcerns
                        .FirstOrDefaultAsync(x => x.Id == ticketConcern.RequestConcernId, cancellationToken);

                    requestConcern.Remarks = null;
                }

                if (hasChanged)
                {
                    ticketConcern.ModifiedBy = command.Modified_By;
                    ticketConcern.UpdatedAt = DateTime.Now;
                    ticketConcern.IsAssigned = true;

                }

                return ticketConcern;

            }

            private async Task<TicketTransactionNotification> TransactionNotification(TicketConcern ticketConcern, User user,AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.Id} has been assigned",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = command.UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = ticketConcern.Id,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                return addNewTicketTransactionNotification;
            }

            private async Task<TicketTransactionNotification> RequestorTransactionNotification(TicketConcern ticketConcern, User user, AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionOngoing = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.RequestConcernId} is now ongoing",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = command.UserId.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.Ongoing,
                    PathId = ticketConcern.RequestConcernId.Value,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionOngoing);

                return addNewTicketTransactionOngoing;
            }

            private async Task<Result?> AttachmentHandler(TicketConcern ticketConcern, AddRequestConcernReceiverCommand command, CancellationToken cancellationToken )
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
                            TicketConcernId = ticketConcern.Id,
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

                return null;
            }





        }
    }
}
