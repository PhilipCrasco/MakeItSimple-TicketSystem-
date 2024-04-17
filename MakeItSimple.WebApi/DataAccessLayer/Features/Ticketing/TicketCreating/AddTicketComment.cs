using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddTicketComment 
    {
        public class AddTicketCommentCommand : IRequest<Result>
        {
            public int ? RequestGeneratorId { get; set; }

            public Guid ? UserId { get; set; }
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public List<RequestComment> RequestComments { get; set; }

            public class RequestComment
            {
                public int ?  TicketCommentId { get; set; }
                public string Comment {  get; set; }
            }

        }

        public class Handler : IRequestHandler<AddTicketCommentCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddTicketCommentCommand command, CancellationToken cancellationToken)
            {
                var prohibitedList = new List<string>();

                var requestGeneratorExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId);
                if (requestGeneratorExist is null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                foreach(var comment in command.RequestComments)
                {

                    var commentExist = await _context.TicketComments
                        .Where(x => x.Id == comment.TicketCommentId).FirstOrDefaultAsync(cancellationToken);

                    var contains = ProhibitedInumerable.Prohibited.FirstOrDefault(word => comment.Comment.ToLower().Contains(word));

                    if (commentExist != null)
                    {
                        if (commentExist.AddedBy != command.UserId)
                        {
                            return Result.Failure(TicketRequestError.NotAutorizeToEdit());
                        }

                        bool IsChange = false;

                        if( commentExist.Comment != comment.Comment)
                        {
                            commentExist.Comment = comment.Comment;
                        }

                        if (contains != null)
                        {
                            return Result.Failure(TicketRequestError.ProhibitedWord(contains));
                        }

                        if (IsChange)
                        {
                            commentExist.ModifiedBy = command.Modified_By;
                        }
                        
                    }
                    else
                    {

                        var addComment = new TicketComment
                        {
                            RequestGeneratorId = command.RequestGeneratorId,
                            Comment = comment.Comment,
                            AddedBy = command.Added_By,
                            IsClicked = false
                        };


                        if (contains != null)
                        {
                            return Result.Failure(TicketRequestError.ProhibitedWord(contains));
                        }

                        await _context.TicketComments.AddAsync(addComment);

                    }
                }

                await _context.SaveChangesAsync();
                return Result.Success();
            }
        }
    }
}
