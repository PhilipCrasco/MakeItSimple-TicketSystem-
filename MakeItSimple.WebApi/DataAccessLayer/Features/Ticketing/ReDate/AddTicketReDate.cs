using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using MakeItSimple.WebApi.Models.Ticketing;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Cloudinary;
using Microsoft.Extensions.Options;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate
{
    public class AddTicketReDate
    {
        public class AddTicketReDateCommand : IRequest<Result>
        {
            public string ReDate_Remarks { get; set; }
            public Guid? Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public Guid ? Transacted_By { get; set; }
            public Guid ? ReDate_By { get; set; }
            public int? TicketConcernId { get; set; }
            public int? TicketReDateId { get; set; }

            public DateTime ? Start_Date {  get; set; }

            public DateTime? Target_Date { get; set; }
            public List<AddReDateAttachment> AddReDateAttachments { get; set; }

            public class AddReDateAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }

            }

        }

        public class Handler : IRequestHandler<AddTicketReDateCommand, Result>
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



            public async Task<Result> Handle(AddTicketReDateCommand command, CancellationToken cancellationToken)
            {
                var dateNow = DateTime.Today;

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);

                if (ticketConcernExist == null)
                {
                    return Result.Failure(ReDateError.TicketConcernIdNotExist());
                }

                if(command.Start_Date > command.Target_Date || command.Target_Date < dateNow)
                {
                    return Result.Failure(ReDateError.DateTimeInvalid());         
                }

                var approverList = await _context.Approvers
               .Include(x => x.User)
               .Where(x => x.SubUnitId == ticketConcernExist.User.SubUnitId)
               .ToListAsync();

                if (approverList == null)
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                var reDateExist = await _context.TicketReDates
                    .FirstOrDefaultAsync(x => x.Id == command.TicketReDateId, cancellationToken);

                if (reDateExist is not null)
                {

                    bool isChange = false;

                    if(reDateExist.StartDate != command.Start_Date)
                    {
                        reDateExist.StartDate = command.Start_Date;
                        isChange = true;
                    }

                    if (reDateExist.TargetDate != command.Target_Date)
                    {
                        reDateExist.TargetDate = command.Target_Date;
                        isChange = true;
                    }

                    if (reDateExist.ReDateRemarks != command.ReDate_Remarks)
                    {
                        reDateExist.ReDateRemarks = command.ReDate_Remarks;
                        isChange = true;
                    }

                    if(isChange)
                    {
                        reDateExist.ModifiedBy = command.Modified_By;
                        reDateExist.UpdatedAt = DateTime.Now;
                    }

                }
                else
                {

                    var approverUser = approverList
                        .FirstOrDefault(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                    ticketConcernExist.IsReDate = false;

                    var addReDate = new TicketReDate
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        ReDateRemarks = command.ReDate_Remarks,
                        StartDate = command.Start_Date,
                        TargetDate = command.Target_Date,
                        AddedBy = command.Added_By,
                        IsReDate = false,
                        ReDateBy = command.ReDate_By,
                        TicketApprover = approverUser.UserId,
                    };

                    await _context.TicketReDates.AddAsync(addReDate);
                    await _context.SaveChangesAsync(cancellationToken);

                    reDateExist = addReDate;

                    foreach (var approver in approverList)
                    {
                        var addNewApprover = new ApproverTicketing
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            TicketReDateId = addReDate.Id,
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

                        TicketConcernId = ticketConcernExist.Id,
                        TransactedBy = command.Transacted_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.ForReDate,
                        Status = TicketingConString.ReDateRequest

                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                }

                var uploadTasks = new List<Task>();

                if (command.AddReDateAttachments.Count(x => x.Attachment != null) > 0)
                {

                    foreach (var attachments in command.AddReDateAttachments.Where(attachments => attachments.Attachment.Length > 0))
                    {

                        var ticketAttachment = await _context.TicketAttachments
                         .FirstOrDefaultAsync(x => x.Id == attachments.TicketAttachmentId, cancellationToken);

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
                                PublicId = $"MakeITSimple/Ticketing/Transfer/{userDetails.Fullname}/{attachments.Attachment.FileName}"
                            };

                            var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);
                            string attachmentUrl = attachmentResult.SecureUrl.ToString();
                            string transformedUrl = _url.TransformUrlForViewOnly(attachmentUrl, attachments.Attachment.FileName);

                            if (ticketAttachment is not null)
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
                                    TicketReDateId = reDateExist.Id,
                                    Attachment = attachmentResult.SecureUrl.ToString(),
                                    FileName = attachments.Attachment.FileName,
                                    FileSize = attachments.Attachment.Length,
                                    AddedBy = command.Added_By,
                                };

                                await _context.AddAsync(addAttachment, cancellationToken);
                            }

                        }, cancellationToken));

                    }

                    await Task.WhenAll(uploadTasks);
                }

                await _context.SaveChangesAsync(cancellationToken);


                return Result.Success();
            }

        }
    }
}
