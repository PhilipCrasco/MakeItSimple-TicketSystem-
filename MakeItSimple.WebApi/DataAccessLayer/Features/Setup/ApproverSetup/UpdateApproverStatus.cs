using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup
{
    public class UpdateApproverStatus
    {

        public class UpdateApproverStatusCommand : IRequest<Result>
        {
            public ICollection<UpdateApproverStatusId> UpdateApproverStatusIds { get; set; }
            public class UpdateApproverStatusId
            {
                public int ? ChannelId { get; set; }
            }

        }

        public class Handler : IRequestHandler<UpdateApproverStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateApproverStatusCommand command, CancellationToken cancellationToken)
            {


                foreach(var approver in command.UpdateApproverStatusIds)
                {

                    var approverExist = await _context.Approvers.FirstOrDefaultAsync(x => x.ChannelId == approver.ChannelId, cancellationToken);

                    if (approverExist == null)
                    {
                        return Result.Failure(ApproverError.ChannelNotExist());
                    }

                    var approverChannelList = await _context.Approvers.Where(x => x.ChannelId == approver.ChannelId).ToListAsync();

                    foreach(var approverId in approverChannelList)
                    {
                        
                        approverId.IsActive = !approverId.IsActive;
                    }


                }

                await _context.SaveChangesAsync();

                return Result.Success();
            }
        }
    }
}
