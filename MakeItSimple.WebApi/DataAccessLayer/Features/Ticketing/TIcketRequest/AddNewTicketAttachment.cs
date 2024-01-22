using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using System.Net.Mail;
using Microsoft.AspNetCore.Http.HttpResults;
using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest
{


    public class AddNewTicketAttachmentResult
    {
        public int Id { get; set; }
        public int? RequestGeneratorId { get; set; }
        public Guid? Added_By { get; set; }
        public DateTime Created_At { get; set; }
        public string Attachments { get; set; }

    }

    public class AddNewTicketAttachment
    {
        public record AddNewTicketAttachmentCommand : IRequest<Result>
        {
            public int? RequestGeneratorId { get; set; }

            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }

            //public List<IFormFile> Attachments { get; set; }

            public List<Files> Attachments { get; set; }

            public class Files
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }

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

                var ticketIdNotExist = await _context.RequestGenerators
                    .FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId, cancellationToken);
                if (ticketIdNotExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                var getTicketConcern = await _context.TicketConcerns.Include(x => x.Department).Include(x => x.SubUnit)
                    .Include(x => x.User).Include(x => x.RequestGenerator)
                    .FirstOrDefaultAsync(x => x.RequestGeneratorId == command.RequestGeneratorId, cancellationToken);

                var ticketAttachmentList = await _context.TicketAttachments.Where(x => x.RequestGeneratorId == ticketIdNotExist.Id).ToListAsync();

                var uploadTasks = new List<Task>();

                //if (command.Attachments == null)
                //{
                //    return Result.Failure(TicketRequestError.AttachmentNotNull());
                //}

                foreach (var attachments in command.Attachments.Where(attachments => attachments.Attachment.Length > 0))
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
                            PublicId = $"MakeITSimple/{getTicketConcern.Department.DepartmentName}/{getTicketConcern.User.Fullname}/Request/{ticketIdNotExist.Id}/{attachments.Attachment.FileName}"
                        };

                        var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);

                      //var attachmentAlreadyExist = await _context.TicketAttachments.FirstOrDefaultAsync(x => x.Attachment == attachmentResult.SecureUrl.ToString()
                      // && ticketAttachment.Attachment != attachmentResult.SecureUrl.ToString(), cancellationToken);
                      //  if (attachmentAlreadyExist != null)
                      //  {
                      //      errorList.Add(attachmentAlreadyExist);
                      //  }
 

                        if(ticketAttachment != null )
                        {
                            var hasChanged = false;

                            if(ticketAttachment.Attachment != attachmentResult.SecureUrl.ToString())
                            {
                                ticketAttachment.Attachment = attachmentResult.SecureUrl.ToString();
                                hasChanged = true;
                            }

                            if(hasChanged)
                            {
                                ticketAttachment.ModifiedBy = command.Modified_By;
                                ticketAttachment.UpdatedAt = DateTime.Now;
                                attachmentList.Add(ticketAttachment);

                            }

                            if(!hasChanged)
                            {
                                attachmentList.Add(ticketAttachment);
                            }

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
                            attachmentList.Add(addAttachment);

                        }
                        
                    }, cancellationToken));

                }


                await Task.WhenAll(uploadTasks);
                await _context.SaveChangesAsync(cancellationToken);

                var results = attachmentList.Select(x => new AddNewTicketAttachmentResult
                {
                    Id = x.Id,
                    RequestGeneratorId = x.RequestGeneratorId,
                    Attachments = x.Attachment,
                    Added_By = x.AddedBy,
                    Created_At = x.CreatedAt,

                });


                return Result.Success(results);


            }
        }

    }
}
