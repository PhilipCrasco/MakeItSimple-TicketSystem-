using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddCommentNotificationValidator
    {
        public class AddCommentNotificationValidatorCommand : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public Guid Added_By { get; set; }
            public int? TicketConcernId { get; set; }

        }


        public class Handler : IRequestHandler<AddCommentNotificationValidatorCommand,Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddCommentNotificationValidatorCommand command, CancellationToken cancellationToken)
            {

                var ticketConcernExist = await _context.TicketConcerns
                .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);

                if (ticketConcernExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());
                }

                var commentViewList = await _context.TicketCommentViews
                    .Where(x => x.TicketConcernId == command.TicketConcernId && x.UserId == command.Added_By)
                    .ToListAsync();

                var commentViewSelect =  commentViewList.Select(x => x.TicketCommentId);

                var commentExist = await _context.TicketComments
                .Where(x =>!commentViewSelect.Contains( x.Id) && x.IsActive == true && x.TicketConcernId == command.TicketConcernId)
                .ToListAsync();

                foreach (var comment in commentExist)
                {
                    var addCommentView = new TicketCommentView
                    {
                        TicketCommentId = comment.Id,
                        UserId = command.UserId,
                        TicketConcernId = comment.TicketConcernId,
                        IsClicked = true,
                        AddedBy = command.Added_By     
                    };

                    await _context.TicketCommentViews.AddAsync(addCommentView, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }

    }
}
