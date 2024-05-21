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
            public Guid ? Requestor_By { get; set; }
            public Guid ? Transfer_By { get; set; }
            public int ChannelId { get; set; }
            public string TransferRemarks { get; set; }
            public List<AddTransferTicket> AddTransferTickets { get; set; }
            public List<AddTransferAttachment> AddTransferAttachments { get; set; }

            public class AddTransferTicket
            {
                public int? RequestTransactionId { get; set; }
                public Guid? UserId { get; set; }
                public DateTime ? Start_Date { get; set; }
                public DateTime ? Target_Date { get; set; }
            }

            public class AddTransferAttachment
            {
                public IFormFile Attachment { get; set; }
            }
        }

        public class Handler : IRequestHandler<AddNewTransferTicketCommand, Result>
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

            public async Task<Result> Handle(AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var transferList = new List<TransferTicketConcern>();
                var ticketGenerator = new TicketTransaction { IsActive = true };

                await _context.TicketTransactions.AddAsync(ticketGenerator, cancellationToken);

                var channelexist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);
                if (channelexist is null)
                {
                    return Result.Failure(TransferTicketError.ChannelNotExist());
                }

                foreach (var transferConcern in command.AddTransferTickets)
                {

                    var userExist = await _context.Users
                        .FirstOrDefaultAsync(x => x.Id == transferConcern.UserId, cancellationToken);

                    if(userExist is null)
                    {
                        return Result.Failure(TransferTicketError.UserNotExist());
                    }

                    var requestTransactionExist = await _context.RequestTransactions
                        .FirstOrDefaultAsync(x => x.Id == transferConcern.RequestTransactionId, cancellationToken);

                    if (requestTransactionExist != null )
                    {

                        if (command.AddTransferTickets.Count(x => x.RequestTransactionId == transferConcern.RequestTransactionId) > 1)
                        {
                            return Result.Failure(TransferTicketError.DuplicateConcernTicket());
                        }

                        var ticketConcern = await _context.TicketConcerns
                            .Where(x => x.RequestTransactionId == requestTransactionExist.Id
                            && x.UserId == command.Added_By && x.RequestConcernId != null && x.IsActive == true)
                            .FirstOrDefaultAsync();

                        var ticketConcernList = await _context.TicketConcerns
                               .Where(x => x.RequestTransactionId == requestTransactionExist.Id && x.IsActive == true)
                               .ToListAsync();

                        foreach (var concern in ticketConcernList)
                        {
                            concern.IsTransfer = false;
                        }

                        if (ticketConcern.UserId == transferConcern.UserId) 
                        {
                            return Result.Failure(TransferTicketError.InvalidTransferTicket());
                        }

                        var transferTicketAlreadyExist = await _context.TransferTicketConcerns
                            .FirstOrDefaultAsync(x => x.RequestTransactionId == transferConcern.RequestTransactionId
                          && x.IsActive == true && x.IsTransfer == false, cancellationToken);

                        if (transferTicketAlreadyExist != null)
                        {
                            return Result.Failure(TransferTicketError.TransferTicketAlreadyExist());  
                        }

                        await _context.SaveChangesAsync(cancellationToken);

                        var getApproverUser = await _context.Approvers
                            .Include(x => x.User)
                            .Where(x => x.SubUnitId == userExist.SubUnitId)
                            .ToListAsync();
                        
                        var getApproverUserId = getApproverUser.First(x => x.ApproverLevel == getApproverUser.Min(x => x.ApproverLevel));

                        var addTransferTicket = new TransferTicketConcern
                        {
                            TicketTransactionId = ticketGenerator.Id,
                            RequestTransactionId = requestTransactionExist.Id,
                            ChannelId = command.ChannelId,
                            UserId = transferConcern.UserId,
                            ConcernDetails = ticketConcern.ConcernDetails,
                            TransferRemarks = command.TransferRemarks,
                            AddedBy = command.Added_By,
                            //StartDate = ticketConcern.StartDate,
                            //TargetDate = ticketConcern.TargetDate,
                            IsTransfer = false,
                            TicketApprover = getApproverUserId.UserId

                        };

                        transferList.Add(addTransferTicket);
                        await _context.TransferTicketConcerns.AddAsync(addTransferTicket, cancellationToken);

                        await _context.SaveChangesAsync(cancellationToken);

                    }
                    else
                    {
                        return Result.Failure(TransferTicketError.TicketIdNotExist());
                    }

                    var uploadTasks = new List<Task>();

                    if (command.AddTransferAttachments.Count(x => x.Attachment != null) > 0)
                    {
                        foreach (var attachments in command.AddTransferAttachments.Where(attachments => attachments.Attachment.Length > 0))
                        {

   
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
                                    PublicId = $"MakeITSimple/Ticketing/Request/{ticketGenerator.Id}/{attachments.Attachment.FileName}"
                                };

                                var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);

                                    var addAttachment = new TicketAttachment
                                    {
                                        TicketTransactionId = ticketGenerator.Id,
                                        Attachment = attachmentResult.SecureUrl.ToString(),
                                        FileName = attachments.Attachment.FileName,
                                        FileSize = attachments.Attachment.Length,
                                        AddedBy = command.Added_By,
                                    };

                                    await _context.AddAsync(addAttachment, cancellationToken);

                            }, cancellationToken));

                        }

                        await Task.WhenAll(uploadTasks);
                    }

                }

                var getApprover = await _context.Approvers.Include(x => x.User)
                    .Where(x => x.SubUnitId == transferList.First().User.SubUnitId).ToListAsync();


                if (getApprover == null)
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                foreach (var approver in getApprover)
                {
                    var addNewApprover = new ApproverTicketing
                    {
                        TicketTransactionId = ticketGenerator.Id,
                        SubUnitId = approver.SubUnitId,
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
                    
                   TicketTransactionId = ticketGenerator.Id,
                   RequestorBy = command.Requestor_By,
                   TransactionDate = DateTime.Now,
                   Request = TicketingConString.Transfer,
                   Status = TicketingConString.RequestCreated

                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }

    }
}
 