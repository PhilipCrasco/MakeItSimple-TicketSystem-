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
            public int? RequestGeneratorId { get; set; }

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

                var requestGeneratorExist = await _context.RequestGenerators
                .FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId, cancellationToken);

                if (requestGeneratorExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                var commentExist = await _context.TicketComments
                .Where(x => x.RequestGeneratorId == requestGeneratorExist.Id && x.IsActive == true).ToListAsync();

                foreach (var comment in commentExist)
                {
                    var addCommentView = new TicketCommentView
                    {
                        TicketCommentId = comment.Id,
                        UserId = command.UserId,
                        RequestGeneratorId = comment.RequestGeneratorId,
                        IsClicked = true,
                    };

                    await _context.TicketCommentViews.AddAsync(addCommentView, cancellationToken);

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }

    }
}
