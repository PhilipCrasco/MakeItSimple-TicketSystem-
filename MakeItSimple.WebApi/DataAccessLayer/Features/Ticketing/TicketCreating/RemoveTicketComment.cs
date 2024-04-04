using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class RemoveTicketComment
    {
        public class RemoveTicketCommentCommand : IRequest<Result>
        {
            public Guid? UserId { get; set; }

            public List<RemoveComment> RemoveComments { get; set; }
            public class RemoveComment
            {
                public int TicketCommentId {  get; set; }
            }
        }

        public class Handler : IRequestHandler<RemoveTicketCommentCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RemoveTicketCommentCommand command, CancellationToken cancellationToken)
            {
                
                foreach (var comment in command.RemoveComments)
                {
                    var ticketNotExist = await _context.TicketComments
                        .FirstOrDefaultAsync(x => x.IsActive == true && x.Id == comment.TicketCommentId, cancellationToken);

                    if (ticketNotExist.AddedBy != command.UserId)
                    {
                        return Result.Failure(TicketRequestError.NotAutorizeToDelete());
                    }

                    if (ticketNotExist == null)
                    {
                        return Result.Failure(TicketRequestError.TicketCommentNotExist());
                    }

                    ticketNotExist.IsActive = false;
                   
                }

                 await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
