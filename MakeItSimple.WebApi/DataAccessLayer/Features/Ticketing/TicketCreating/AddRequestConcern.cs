using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddRequestConcern
    {

        public class AddRequestConcernCommand : IRequest<Result>
        {
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public Guid ? UserId { get; set; }
            public int? RequestConcernId { get; set; }
            public string Concern { get; set; }

        }

        public class Handler : IRequestHandler<AddRequestConcernCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddRequestConcernCommand command, CancellationToken cancellationToken)
            {

                var userId = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId);
                if (userId == null) 
                {
                    return Result.Failure(UserError.UserNotExist());
                }

                var requestConcernIdExist = await _context.RequestConcerns.FirstOrDefaultAsync(x => x.Id == command.RequestConcernId, cancellationToken);
                 if (requestConcernIdExist != null) 
                 {
                    var ticketConcernExist = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.RequestConcernId == requestConcernIdExist.Id, cancellationToken);

                    bool isChange = false;

                    if(requestConcernIdExist.Concern != command.Concern)
                    {
                        requestConcernIdExist.Concern = command.Concern;
                        isChange = true;
                    }

                    if (isChange)
                    {
                        requestConcernIdExist.ModifiedBy = command.Modified_By;
                        ticketConcernExist.ConcernDetails = requestConcernIdExist.Concern;
                    }

                }
                 else
                 {

                    var requestGeneratorId = new RequestGenerator { IsActive = true };

                    await _context.RequestGenerators.AddAsync(requestGeneratorId);
                    await _context.SaveChangesAsync(cancellationToken);

                    var addRequestConcern = new RequestConcern
                    {
                        RequestGeneratorId = requestGeneratorId.Id,
                        UserId = userId.Id,
                        Concern = command.Concern,
                        AddedBy = command.Added_By,
                        ConcernStatus = TicketingConString.ForApprovalTicket,
                        IsDone = false
                        
                    };

                    await _context.RequestConcerns.AddAsync(addRequestConcern);
                    await _context.SaveChangesAsync(cancellationToken);


                    var addTicketConcern = new TicketConcern
                    {
                        RequestGeneratorId = requestGeneratorId.Id,
                        RequestConcernId = addRequestConcern.Id,
                        RequestorBy = command.UserId,
                        ConcernDetails = addRequestConcern.Concern,
                        IsApprove = false,
                        AddedBy = command.Added_By,
                        TicketType = TicketingConString.Concern,
                        ConcernStatus = addRequestConcern.ConcernStatus


                    };

                    await _context.TicketConcerns.AddAsync(addTicketConcern);

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }
        }
    }
}
