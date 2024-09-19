using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.RejectRequestTicket.RejectRequestTicketCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class RejectRequestTicket
    {

        public class RejectRequestTicketCommand : IRequest<Result>
        {
            public int TicketConcernId { get; set; }
            public string Role { get; set; }
            public Guid? Reject_By { get; set; }
            public string Remarks { get; set; }

        }

        public class Handler : IRequestHandler<RejectRequestTicketCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectRequestTicketCommand command, CancellationToken cancellationToken)
            {

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);

                if (ticketConcernExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                var userRoleList = await _context.UserRoles.ToListAsync();

                var receiverPermission = userRoleList
                .Where(x => x.Permissions.Contains(TicketingConString.Receiver))
                .Select(x => x.UserRoleName);

                if (!receiverPermission.Any(x => x.Contains(command.Role)))
                {
                    return Result.Failure(TicketRequestError.UnAuthorizedReceiver());
                }

                ticketConcernExist.IsReject = true;
                ticketConcernExist.Remarks = command.Remarks;      

                var requestConcernExist = await _context.RequestConcerns
                    .FirstOrDefaultAsync(x => x.Id == ticketConcernExist.RequestConcernId,cancellationToken);
        
                requestConcernExist.IsReject = true;
                requestConcernExist.Remarks = command.Remarks;
                requestConcernExist.RejectBy = command.Reject_By;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
