using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class GetRequestAttachment
    {
        public class GetRequestAttachmentResult
        {
            public int ? TicketConcernId { get; set; }

            public List<TicketAttachment> Attachments { get; set; }

            public class TicketAttachment
            {
                public int TicketAttachmentId { get; set; }
                public string Attachment { get; set; }
                public string FileName { get; set; }

                public decimal ? FileSize {  get; set; }

                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }

                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }

            }

        }


        public class GetRequestAttachmentQuery : IRequest<Result>
        {
            public int? Id { get; set; }
            public bool ? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetRequestAttachmentQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetRequestAttachmentQuery request, CancellationToken cancellationToken)
            {
                IQueryable<TicketAttachment>  ticketAttachmentQuery = _context.TicketAttachments
                    .AsNoTracking()
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.AddedByUser)
                    .ThenInclude(x => x.ModifiedByUser);

                if (request.Id != null)
                {
                    ticketAttachmentQuery = ticketAttachmentQuery.Where(x => x.TicketConcernId == request.Id);
                }


                if (request.Status != null)
                {
                    ticketAttachmentQuery = ticketAttachmentQuery.Where(x => x.IsActive == request.Status);
                }


                var results = await ticketAttachmentQuery.GroupBy(x => x.TicketConcernId)
                    .Select(x => new GetRequestAttachmentResult
                {
                    TicketConcernId = x.Key,
                    Attachments = x.Select(x => new GetRequestAttachmentResult.TicketAttachment
                    {
                        TicketAttachmentId = x.Id,
                        Attachment = x.Attachment,
                        FileName = x.FileName,
                        FileSize = x.FileSize,
                        Added_By = x.AddedByUser.Fullname, 
                        Created_At = x.CreatedAt,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt

                    }).ToList(),


                }).ToListAsync();

                return Result.Success(results);

            }
        }


    }

}
