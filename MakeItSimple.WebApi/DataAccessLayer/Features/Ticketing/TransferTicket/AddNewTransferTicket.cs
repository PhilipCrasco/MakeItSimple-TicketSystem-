using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.AddNewTransferTicket.AddNewTransferTicketCommand;
using MakeItSimple.WebApi.Common.Cloudinary;
using Microsoft.Extensions.Options;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class AddNewTransferTicket
    {
        public class AddNewTransferTicketCommand : IRequest<Result>
        {
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public Guid ? Transacted_By { get; set; }
            public Guid ? Transfer_By { get; set; }
            public int? TransferTicketId { get; set; }
            public int ? TicketConcernId { get; set; }
            public string TransferRemarks { get; set; }
            public string Modules {  get; set; }
            public List<AddTransferAttachment> AddTransferAttachments { get; set; }

            public class AddTransferAttachment
            {
                public int ? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }
        }

        public class Handler : IRequestHandler<AddNewTransferTicketCommand, Result>
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

            public async Task<Result> Handle(AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);
               
                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);

                if (ticketConcernExist is null)
                {
                    return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                }

                if (ticketConcernExist.IsTransfer is not null)
                {
                    if (ticketConcernExist.IsReDate is false || ticketConcernExist.IsReTicket is false
                        || ticketConcernExist.IsClosedApprove is not null)
                    {
                        return Result.Failure(TransferTicketError.TransferInvalid());
                    }
                }

                var approverList = await _context.Approvers
               .Include(x => x.User)
               .Where(x => x.SubUnitId == ticketConcernExist.User.SubUnitId)
               .ToListAsync();

                if (approverList == null)
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                var transferTicketExist = await _context.TransferTicketConcerns
                        .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist is not null)
                {
                    var isChange = false;

                    if(transferTicketExist.TransferRemarks != command.TransferRemarks)
                    {
                        transferTicketExist.TransferRemarks = command.TransferRemarks;
                    }

                    if(isChange)
                    {
                        transferTicketExist.ModifiedBy = command.Modified_By;
                        transferTicketExist.UpdatedAt = DateTime.Now;
                    }

                }
                else
                {

                    var approverUser = approverList
                        .FirstOrDefault(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                    ticketConcernExist.IsTransfer = false;

                    var addTransferTicket = new TransferTicketConcern
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        TransferRemarks = command.TransferRemarks,
                        TransferBy = command.Transfer_By,
                        IsTransfer = false,
                        AddedBy = command.Added_By,
                        TicketApprover = approverUser.UserId,

                    };

                    await _context.TransferTicketConcerns.AddAsync(addTransferTicket);
                    await _context.SaveChangesAsync(cancellationToken);

                    transferTicketExist = addTransferTicket;

                    foreach (var approver in approverList)
                    {
                        var addNewApprover = new ApproverTicketing
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            TransferTicketConcernId = addTransferTicket.Id,
                            UserId = approver.UserId,
                            ApproverLevel = approver.ApproverLevel,
                            AddedBy = command.Added_By,
                            CreatedAt = DateTime.Now,
                            Status = "Transfer",

                        };

                        await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);
                    }

                    var addTicketHistory = new TicketHistory
                    {

                        TicketConcernId = ticketConcernExist.Id,
                        TransactedBy = command.Transacted_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.ForTransfer,
                        Status = TicketingConString.TransferRequest

                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                    foreach(var approver in approverList)
                    {

                        var approverLevel = approver.ApproverLevel == 1 ? $"{approver.ApproverLevel}st"
                            : approver.ApproverLevel == 2 ? $"{approver.ApproverLevel}nd"
                            : approver.ApproverLevel == 3 ? $"{approver.ApproverLevel}rd"
                            : $"{approver.ApproverLevel}th";


                        var addApproverHistory = new TicketHistory
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            TransactedBy = approver.UserId,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.Approval,
                            Status = $"{TicketingConString.TransferForApproval} {approverLevel} Approver",
                            Approver_Level = approver.ApproverLevel,
                            
                        };

                        await _context.TicketHistories.AddAsync(addApproverHistory, cancellationToken);
                    }


                    var addNewTicketTransactionNotification = new TicketTransactionNotification
                    {

                        Message = $"Ticket number {ticketConcernExist.Id} is pending for transfer approval",
                        AddedBy = userDetails.Id,
                        Created_At = DateTime.Now,
                        ReceiveBy = addTransferTicket.TicketApprover.Value,
                        Modules = command.Modules,
                        PathId = ticketConcernExist.Id

                    };

                    await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                }


                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.AddTransferAttachments.Count(x => x.Attachment != null) > 0)
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
                                TicketConcernId = ticketConcernExist.Id,
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
 