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
            public int CommentTicketCount { get; set; }
        }

        public class CommentTicketNotificationQuery
        {
            public int? TicketConcernId { get; set; }

            public int ? TicketCommentId { get; set; }

            public bool IsActive { get; set; }

            

        }

     

        public class CommentNotificationQueryResult : IRequest<Result>
        {
            public int ? TicketConcernId { get; set; }
            public bool ? Status { get; set; }
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
                var query = await _context.TicketComments
                    .Include(x => x.TicketCommentViews)
                    .Where(x => x.TicketConcernId == request.TicketConcernId && x.AddedBy == request.UserId)
                    .ToListAsync();

                if (request.Status != null)
                {
                    query = query
                        .Where(x => x.IsActive == request.Status)
                        .ToList();
                }

                var ticketComment = await _context.TicketCommentViews
                    .Where(x => x.TicketConcernId == request.TicketConcernId 
                    && x.UserId == request.UserId)
                    .ToListAsync();

                var ticketCommentSelect = ticketComment.Select(x => x.TicketCommentId);

                query = query
                    .Where(x => !ticketCommentSelect.Contains(x.Id))
                    .ToList();

                var notification = query.Select(x => new CommentTicketNotificationResult
                {
                    CommentTicketCount = query.Count()

                }).DistinctBy(x => x.CommentTicketCount)
                .ToList();
         

                return Result.Success(notification);
            }
        }
    }
}
