using DocumentFormat.OpenXml.InkML;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.CreateTransfer
{
    public partial class AddNewTransferTicket
    {

        public class Handler : IRequestHandler<AddNewTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);


               var validation = await ValidationHandler(ticketConcernExist,command, cancellationToken);
                if(validation is not null)
                    return validation;
                
                var transferTicketExist = await _context.TransferTicketConcerns
                        .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId);

                if (transferTicketExist is not null)
                {
                    await UpdateTransferTicket(transferTicketExist,command ,cancellationToken);

                }
                else
                {

                    var addTransferTicket = await CreateTransferTicket(transferTicketExist,ticketConcernExist, command, cancellationToken);

                    transferTicketExist = addTransferTicket;

                    await CreateApprover(ticketConcernExist,transferTicketExist,command ,cancellationToken);
                    await CreateHistory(ticketConcernExist,command,cancellationToken);

                }

                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.AddTransferAttachments.Count(x => x.Attachment != null) > 0)
                {
                    var attachment = await CreateAttachment(transferTicketExist,command,cancellationToken);
                    if(attachment is not null) 
                        return attachment;
 
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


            private async Task<Result?> ValidationHandler(TicketConcern ticketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                if (ticketConcern is null)
                    return Result.Failure(TransferTicketError.TicketConcernIdNotExist());


                if (ticketConcern.IsTransfer is not null && ticketConcern.IsClosedApprove is not null)
                    return Result.Failure(TransferTicketError.TransferInvalid());

                return null;
            }

            private async Task UpdateTransferTicket (TransferTicketConcern transferTicketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var isChange = false;

                if (transferTicketConcern.TransferRemarks != command.TransferRemarks)
                {
                    transferTicketConcern.TransferRemarks = command.TransferRemarks;
                }

                if (transferTicketConcern.Current_Target_Date != command.Current_Target_Date)
                {
                    transferTicketConcern.Current_Target_Date = command.Current_Target_Date;
                }


                if (isChange)
                {
                    transferTicketConcern.ModifiedBy = command.Modified_By;
                    transferTicketConcern.UpdatedAt = DateTime.Now;
                }

            }

            private async Task<TransferTicketConcern> CreateTransferTicket(TransferTicketConcern transferTicketConcern,TicketConcern ticketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                ticketConcern.IsTransfer = false;

                var addTransferTicket = new TransferTicketConcern
                {
                    TicketConcernId = ticketConcern.Id,
                    TransferRemarks = command.TransferRemarks,
                    TransferBy = command.Transfer_By,
                    IsTransfer = false,
                    AddedBy = command.Added_By,
                    TransferTo = command.Transfer_To,
                    Current_Target_Date = ticketConcern.TargetDate,
                    TicketApprover = command.Transfer_To,
                };

                await _context.TransferTicketConcerns.AddAsync(addTransferTicket);
                await _context.SaveChangesAsync(cancellationToken);

                return addTransferTicket;

            }

            private async Task CreateApprover(TicketConcern ticketConcern,TransferTicketConcern transferTicketConcern, AddNewTransferTicketCommand command,CancellationToken cancellationToken)
            {
                var addApprover = new ApproverTicketing
                {
                    TicketConcernId = ticketConcern.Id,
                    TransferTicketConcernId = transferTicketConcern.Id,
                    UserId = command.Transfer_To,
                    ApproverLevel = 1,
                    AddedBy = command.Added_By,
                    CreatedAt = DateTime.Now,
                    Status = TicketingConString.Transfer,

                };

                await _context.ApproverTicketings.AddAsync(addApprover, cancellationToken);

            }

            private async Task CreateHistory(TicketConcern ticketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {

                    TicketConcernId = ticketConcern.Id,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.ForTransfer,
                    Status = TicketingConString.TransferRequest

                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                var addApproverHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcern.Id,
                    TransactedBy = command.Transfer_To,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Approval,
                    Status = $"{TicketingConString.TransferForApproval} 1st Approver",
                    Approver_Level = 1,

                };

                await _context.TicketHistories.AddAsync(addApproverHistory, cancellationToken);

            }

            private async Task CreateTransactionNotification(TicketConcern ticketConcern, User user, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.Id} is pending for transfer approval",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = command.Transfer_To.Value,
                    Modules = PathConString.Approval,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = ticketConcern.Id,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

            }

            private async Task<Result?> CreateAttachment(TransferTicketConcern transferTicketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                foreach (var attachments in command.AddTransferAttachments.Where(a => a.Attachment.Length > 0))
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
                            TicketConcernId = command.TicketConcernId,
                            TransferTicketConcernId = transferTicketConcern.Id,
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
