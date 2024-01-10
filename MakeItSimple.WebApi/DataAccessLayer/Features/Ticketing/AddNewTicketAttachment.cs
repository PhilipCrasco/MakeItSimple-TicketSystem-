using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing.TicketRequest;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using System.Net.Mail;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing
{


    public class AddNewTicketAttachmentResult
    {
        public int  Id { get; set; }
        public int ? TicketTransactionId { get; set; }
        public Guid? Added_By { get; set; }
        public DateTime Created_At { get; set; }
        public string Attachments { get; set; }


    }


    public class AddNewTicketAttachment
    {
        public record AddNewTicketAttachmentCommand : IRequest<Result>
        {
            public int ? TicketTransactionId { get; set; }

            public Guid ? Added_By {  get; set; }

            public List<IFormFile> Attachments { get; set; }

        }

        public class Handler : IRequestHandler<AddNewTicketAttachmentCommand, Result>
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


            public async Task<Result> Handle(AddNewTicketAttachmentCommand command, CancellationToken cancellationToken)
            {
                var attachmentList = new List<TicketAttachment>();

                var ticketIdNotExist = await _context.TicketTransactions.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == command.TicketTransactionId, cancellationToken);
                if (ticketIdNotExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                var getTicketConcern = await _context.TicketConcerns.Include(x => x.Channel)
                    .FirstOrDefaultAsync(x => x.TicketTransactionId == command.TicketTransactionId, cancellationToken);

                var uploadTasks = new List<Task>();

                if (command.Attachments == null)
                {
                    return Result.Failure(TicketRequestError.AttachmentNotNull());
                }

                foreach (var attachments in command.Attachments)
                {
                    if (attachments == null || attachments.Length == 0)
                    {
                        return Result.Failure(TicketRequestError.AttachmentNotNull());
                    }

                    if(attachments.Length > 10 * 1024 * 1024)
                    {
                        return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                    }

                    var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png" };
                    var extension = Path.GetExtension(attachments.FileName)?.ToLowerInvariant();

                    if (extension == null || !allowedFileTypes.Contains(extension))
                    {
                        return Result.Failure(TicketRequestError.InvalidAttachmentType());
                    }

                    uploadTasks.Add(Task.Run(async () =>
                    {

                        await using var stream = attachments.OpenReadStream();

                        var attachmentsParams = new ImageUploadParams
                        {
                            File = new FileDescription(attachments.FileName, stream),
                            PublicId = $"MakeITSimple/{ticketIdNotExist.Department.DepartmentName}/{getTicketConcern.Channel.ChannelName}/Transaction Ticket/{getTicketConcern.TicketTransactionId}/{attachments.FileName}"
                        };

                        var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);

                        var addAttachment = new TicketAttachment
                        {
                            TicketTransactionId = command.TicketTransactionId,
                            Attachment = attachmentResult.SecureUrl.ToString(),
                            AddedBy = command.Added_By,
                        };


                        await _context.AddAsync(addAttachment, cancellationToken);
                        attachmentList.Add(addAttachment);

                    }, cancellationToken));

                }

                await Task.WhenAll(uploadTasks);
                await _context.SaveChangesAsync(cancellationToken);


                var results = attachmentList.Select(x => new AddNewTicketAttachmentResult
                {
                    Id = x.Id,
                    TicketTransactionId = x.TicketTransactionId,
                    Attachments = x.Attachment,
                    Added_By = x.AddedBy,
                    Created_At = x.CreatedAt,

                });


                return Result.Success(results);


            }
        }

    }
}
