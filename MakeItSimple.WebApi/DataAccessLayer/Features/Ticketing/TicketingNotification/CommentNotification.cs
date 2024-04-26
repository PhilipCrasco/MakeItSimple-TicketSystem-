using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class CommentNotification
    {
        public class CommentTicketNotificationResult
        {
            public int ClosingTicketCount { get; set; }
        }

        public class CommentTicketNotificationQuery
        {
            public int? TicketGeneratorId { get; set; }

        }

        public class CommentNotificationQueryResult : IRequest<Result>
        {
            public int ? RequestGeneratorId { get; set; }
            public Guid ? UserId { get; set; }

        }


        public class Handler : IRequestHandler<CommentNotificationQueryResult, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CommentNotificationQueryResult request, CancellationToken cancellationToken)
            {
                var query = await _context.TicketComments.Include(x => x.TicketCommentViews)
                    .Where(x => x.RequestGeneratorId == request.RequestGeneratorId).ToListAsync();

                  

                return Result.Success(request);
            }
        }
    }
}
