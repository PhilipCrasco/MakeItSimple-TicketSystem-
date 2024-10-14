using DocumentFormat.OpenXml.Bibliography;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.AddClosingTicket;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.CreateOnHold
{
    public partial class CreateOnHoldTicket
    {

        public class Handler : IRequestHandler<CreateOnHoldTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CreateOnHoldTicketCommand command, CancellationToken cancellationToken)
            {
                var ticketConcernExist = await _context.TicketConcerns
                    .Include(i => i.RequestorByUser)
                    .FirstOrDefaultAsync(t => t.Id == command.TicketConcernId,cancellationToken);

                if (ticketConcernExist is null)
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());

                var onHoldExist = await _context.TicketOnHolds
                    .FirstOrDefaultAsync(o => o.Id == command.Id,cancellationToken);

                if (onHoldExist is not null)
                {

                    onHoldExist.Reason = command.Reason;
                    ticketConcernExist.OnHoldReason = command.Reason;

                }
                else
                {
                    ticketConcernExist.OnHoldAt = DateTime.Now;
                    ticketConcernExist.OnHoldReason = command.Reason;

                    var addOnHold = await CreateOnHold(command, cancellationToken);
                    await OnHoldTicketHistory(command, cancellationToken);
                    await TransactionNotification(ticketConcernExist, command, cancellationToken);

                    onHoldExist = addOnHold;
                }

                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.OnHoldAttachments.Count(x => x.Attachment != null) > 0)
                {
                    await AttachmentHandler(onHoldExist, command, cancellationToken);
                }


                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


            private async Task<TicketOnHold> CreateOnHold(CreateOnHoldTicketCommand command , CancellationToken cancellationToken)
            {
                var addOnHold = new TicketOnHold
                {
                   TicketConcernId = command.TicketConcernId,
                   Reason = command.Reason,
                   AddedBy = command.Added_By,
                   IsHold = true,

                };
                await _context.TicketOnHolds.AddAsync(addOnHold);

                await _context.SaveChangesAsync(cancellationToken);

                return addOnHold;

            }

            private async Task<TicketHistory> OnHoldTicketHistory(CreateOnHoldTicketCommand command , CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = command.Id,
                    TransactedBy = command.Added_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.OnHold,
                    Status = TicketingConString.OnHoldRequest

                };

                await _context.TicketHistories.AddAsync(addTicketHistory);

                return addTicketHistory;


            }

            private async Task<TicketTransactionNotification> TransactionNotification(TicketConcern ticketConcern, CreateOnHoldTicketCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {command.TicketConcernId} is on-hold",
                    AddedBy = command.Added_By.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = ticketConcern.RequestorBy.Value,
                    //Modules = PathConString.Approval,
                    //Modules_Parameter = PathConString.ForClosingTicket,
                    PathId = command.TicketConcernId,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                return addNewTicketTransactionNotification;
            }
            private async Task<Result?> AttachmentHandler(TicketOnHold onHold, CreateOnHoldTicketCommand command, CancellationToken cancellationToken)
            {

                foreach (var attachments in command.OnHoldAttachments.Where(a => a.Attachment.Length > 0))
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

                    if (ticketAttachment is not null)
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
                            ClosingTicketId = onHold.Id,
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
