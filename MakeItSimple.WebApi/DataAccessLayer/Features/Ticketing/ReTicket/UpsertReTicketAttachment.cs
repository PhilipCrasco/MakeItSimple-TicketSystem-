using CloudinaryDotNet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.Extensions.Options;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class UpsertReTicketAttachment
    {
        public class UpsertReTicketAttachmentCommand : IRequest<Result>
        {
            public int? RequestGeneratorId { get; set; }

            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }

            public List<ReTicketFile> ReTicketFiles { get; set; }

            public class ReTicketFile
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }

            

        }

        public class Handler : IRequestHandler<UpsertReTicketAttachmentCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly Cloudinary _cloudinary;

            public Handler(MisDbContext context, IOptions<CloudinaryOption> options)
            {
                _context = context;
                var account = new Account(
                   options.Value.Cloudname,
                   options.Value.ApiKey,
                   options.Value.ApiSecret);
                _cloudinary = new Cloudinary(account);
            }

            public async Task<Result> Handle(UpsertReTicketAttachmentCommand request, CancellationToken cancellationToken)
            {


                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
