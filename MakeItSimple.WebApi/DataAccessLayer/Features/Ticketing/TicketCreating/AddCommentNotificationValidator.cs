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

            public List<CommentValidator> CommentValidators { get; set; }
            public class CommentValidator
            {
                public int ? TicketCommentId { get; set; }
            }

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

                foreach(var comment in command.CommentValidators)
                {
                    var commentExist = await _context.TicketComments
                        .FirstOrDefaultAsync(x => x.Id == comment.TicketCommentId && x.IsActive == true);

                    if(commentExist == null)
                    {
                        return Result.Failure(TicketRequestError.TicketCommentNotExist());
                    }

                    var viewCommentExist = await _context.TicketCommentViews
                        .FirstOrDefaultAsync(x => x.Id == commentExist.Id && x.UserId == command.UserId);

                    if(viewCommentExist == null)
                    {
                        var addCommentView = new TicketCommentView
                        {
                            TicketCommentId = comment.TicketCommentId,
                            UserId = command.UserId,
                        };

                        await _context.TicketCommentViews.AddAsync(addCommentView);

                    }
                }


                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }

    }
}
