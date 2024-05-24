using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class RequestApprovalReceiver
    {

        public class RequestApprovalReceiverCommand : IRequest<Result>
        {
            public Guid? Approved_By { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
            public int? RequestTransactionId { get; set; }
            public List<Concern> Concerns { get; set; }

            public  class Concern
            {
                public Guid? IssueHandler { get; set; }
            }
        }


        public class Handler : IRequestHandler<RequestApprovalReceiverCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RequestApprovalReceiverCommand request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var allUserList = await _context.UserRoles.ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var requestTransactionExist = await _context.RequestTransactions
                    .FirstOrDefaultAsync(x => x.Id == request.RequestTransactionId, cancellationToken);

                if (requestTransactionExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }




                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
