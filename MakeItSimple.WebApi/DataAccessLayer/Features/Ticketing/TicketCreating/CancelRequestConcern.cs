using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class CancelRequestConcern
    {
        public class CancelRequestConcernCommand : IRequest<Result>
        {
            public List<CancelRequest> CancelRequests { get; set; }
            public class CancelRequest
            {
               
                public int? RequestGeneratorId { get; set; }
                public List<RequestAttachment> RequestAttachments { get; set; }

                public class RequestAttachment
                {
                    public int? TicketAttachmentId { get; set;}
                }
            }

        }

        public class Handler : IRequestHandler<CancelRequestConcernCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelRequestConcernCommand command, CancellationToken cancellationToken)
            {
                foreach(var cancelRequestTransaction in command.CancelRequests)
                {
                    var requestGenaratorExist = await _context.RequestGenerators
                        .FirstOrDefaultAsync(x => x.Id == cancelRequestTransaction.RequestGeneratorId);

                    if (requestGenaratorExist == null)
                    {
                        return Result.Failure(TicketRequestError.TicketIdNotExist());
                    }

                    if(cancelRequestTransaction.RequestAttachments.Count(x => x.TicketAttachmentId != null) <= 0)
                    {
                        var requestConcernList = await _context.RequestConcerns
                            .Where(x => x.RequestGeneratorId == requestGenaratorExist.Id)
                            .ToListAsync();

                        foreach(var cancelRequest in requestConcernList)
                        {
                            cancelRequest.IsActive = false;
                        }

                        var requestConcernAttachmentList = await _context.RequestConcerns
                            .Where(x => x.RequestGeneratorId == requestGenaratorExist.Id)
                            .ToListAsync();

                        foreach (var cancelAttachment in requestConcernAttachmentList)
                        {
                            cancelAttachment.IsActive = false;
                        }

                    }
                    else
                    {
                        foreach (var cancelAttachment in cancelRequestTransaction.RequestAttachments)
                        {
                            var requestAttachment = await _context.TicketAttachments
                            .Where(x => x.Id == cancelAttachment.TicketAttachmentId)
                            .FirstOrDefaultAsync();

                            if (requestAttachment == null)
                            {
                                return Result.Failure(TicketRequestError.AttachmentNotExist());
                            }

                            requestAttachment.IsActive = false;
                        }
                    }
       

                }
                await _context.SaveChangesAsync(cancellationToken);  
                return Result.Success();
            }
        }
    }
}
