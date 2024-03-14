using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup
{
    public class GetApproverRole
    {
        public class GetApproverRoleResult
        {
            public Guid? UserId { get; set; }
            public string EmpId { get; set; }

            public string FullName { get; set; }
        }


        public class GetApproverRoleQuery : IRequest<Result> 
        { 
         
            public string Role {  get; set; }
            public int ChannelId { get; set; }
        }

        public class Handler : IRequestHandler<GetApproverRoleQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetApproverRoleQuery request, CancellationToken cancellationToken)
            {

                var approver = await _context.Approvers.Where(x => x.ChannelId == request.ChannelId).ToListAsync();

                var selectApprover = approver.Select(x => x.UserId);


                var results = await _context.ChannelUsers
                    .Include(x => x.User)
                    .Where(x => x.ChannelId == request.ChannelId && !selectApprover.Contains(x.UserId) && request.Role == TicketingConString.Approver)
                    .Select(x => new GetApproverRoleResult
                    {
                        UserId = x.UserId,
                        EmpId = x.User.EmpId,
                        FullName = x.User.Fullname

                    }).ToListAsync();



                return Result.Success(results);

            }
        }
    }
}
