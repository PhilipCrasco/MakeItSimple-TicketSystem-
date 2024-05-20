using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Experimental;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddCommentNotificationValidator
    {
        public class AddCommentNotificationValidatorCommand : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public Guid Added_By { get; set; }
            public int? RequestTransactionId { get; set; }

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

                var requestTransactionExist = await _context.RequestTransactions
                .FirstOrDefaultAsync(x => x.Id == command.RequestTransactionId, cancellationToken);

                if (requestTransactionExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                var commentViewList = await _context.TicketCommentViews
                    .Where(x => x.RequestTransactionId == command.RequestTransactionId && x.UserId == command.Added_By)
                    .ToListAsync();

                var commentViewSelect =  commentViewList.Select(x => x.RequestTransactionId);

                var commentExist = await _context.TicketComments
                .Where(x =>!commentViewSelect.Contains( x.RequestTransactionId) && x.IsActive == true)
                .ToListAsync();

                foreach (var comment in commentExist)
                {
                    var addCommentView = new TicketCommentView
                    {
                        TicketCommentId = comment.Id,
                        UserId = command.UserId,
                        RequestTransactionId = comment.RequestTransactionId,
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
