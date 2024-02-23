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

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class UpsertTransferAttachment
    {

        public class UpserTransferAttachmentCommand : IRequest<Result>
        {
            public int? RequestGeneratorId { get; set; }

            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public Guid? Requestor_By { get; set; }

            public List<TransferFile> TransferFiles { get; set; }

            public class TransferFile
            {
                public int ? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }

        }

        public class Handler : IRequestHandler<UpserTransferAttachmentCommand, Result>
        {

            private readonly MisDbContext _context;
            private readonly Cloudinary _cloudinary;

            public Handler(MisDbContext context , IOptions<CloudinaryOption> options)
            {
                _context = context;
                var account = new Account(
                   options.Value.Cloudname,
                   options.Value.ApiKey,
                   options.Value.ApiSecret );
                _cloudinary = new Cloudinary(account);
            }

            public async Task<Result> Handle(UpserTransferAttachmentCommand command, CancellationToken cancellationToken)
            {

                var transferUpdateList = new List<bool>();
                var transferNewList = new List<TicketAttachment>();

                var ticketIdNotExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId, cancellationToken);
                if (ticketIdNotExist == null)
                {
                    return Result.Failure(TransferTicketError.TicketIdNotExist());
                }

                var getTicketConcern = await _context.TransferTicketConcerns.Include(x => x.AddedByUser)
                    .ThenInclude(x => x.Department)
                    .FirstOrDefaultAsync(x => x.RequestGeneratorId == ticketIdNotExist.Id, cancellationToken);

                var ticketAttachmentList = await _context.TicketAttachments.Where(x => x.RequestGeneratorId == ticketIdNotExist.Id).ToListAsync();
                var getTicketConcernList = await _context.TransferTicketConcerns.Include(x => x.AddedByUser).ThenInclude(x => x.Department).Where(x => x.RequestGeneratorId == ticketIdNotExist.Id).ToListAsync();

                var ticketHistoryList = await _context.TicketHistories.Where(x => x.RequestGeneratorId == x.RequestGeneratorId).ToListAsync();
                var ticketHistoryId = ticketHistoryList.FirstOrDefault(x => x.Id == ticketHistoryList.Max(x => x.Id) );

                var uploadTasks = new List<Task>();

                foreach (var attachments in command.TransferFiles.Where(attachments => attachments.Attachment.Length > 0))
                {

                    var ticketAttachment = ticketAttachmentList.FirstOrDefault(x => x.Id == attachments.TicketAttachmentId);

                    if (attachments.Attachment == null || attachments.Attachment.Length == 0)
                    {
                        return Result.Failure(TicketRequestError.AttachmentNotNull());
                    }

                    if (attachments.Attachment.Length > 10 * 1024 * 1024)
                    {
                        return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                    }

                    var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png" };
                    var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                    if (extension == null || !allowedFileTypes.Contains(extension))
                    {
                        return Result.Failure(TicketRequestError.InvalidAttachmentType());
                    }

                    uploadTasks.Add(Task.Run(async () =>
                    {

                        await using var stream = attachments.Attachment.OpenReadStream();

                        var attachmentsParams = new ImageUploadParams
                        {
                            File = new FileDescription(attachments.Attachment.FileName, stream),
                            PublicId = $"MakeITSimple/{getTicketConcern.AddedByUser.Department.DepartmentName}/{getTicketConcern.AddedByUser.Fullname}/Transfer/{ticketIdNotExist.Id}/{attachments.Attachment.FileName}"
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

                            }
                            transferUpdateList.Add(hasChanged);
                        }
                        else
                        {
                            var addAttachment = new TicketAttachment
                            {
                                RequestGeneratorId = command.RequestGeneratorId,
                                Attachment = attachmentResult.SecureUrl.ToString(),
                                AddedBy = command.Added_By,
                            };

                            await _context.AddAsync(addAttachment, cancellationToken);
                            transferNewList.Add(addAttachment);   
                        }
                        
                    }, cancellationToken));

                }

                await Task.WhenAll(uploadTasks);

                if ((getTicketConcernList.First().IsRejectTransfer == true && transferUpdateList.Count > 0) 
                    || (transferNewList.Count > 0 && getTicketConcernList.First().IsRejectTransfer == true))
                {
                    if (ticketHistoryId.Status != TicketingConString.ApproveBy || ticketHistoryId.Status != TicketingConString.RequestCreated)
                    {
                        var addTicketHistory = new TicketHistory
                        {
                            RequestGeneratorId = ticketIdNotExist.Id,
                            RequestorBy = command.Requestor_By,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.Transfer,
                            Status = TicketingConString.RequestUpdate
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                        foreach (var transferTicket in getTicketConcernList)
                        {
                            transferTicket.IsTransfer = false;
                            transferTicket.IsRejectTransfer = false;
                            transferTicket.RejectTransferBy = null;
                            transferTicket.RejectTransferAt = null;

                        };
                    }
                }


                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
