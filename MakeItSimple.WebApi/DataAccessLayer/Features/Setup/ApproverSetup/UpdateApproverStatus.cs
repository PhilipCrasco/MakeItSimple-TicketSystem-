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
        public class UpdateApproverStatusResult
        {
            public int Id { get; set; }
            public bool Status { get; set; }
        }
        public class UpdateApproverStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
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
                
                var approver = await _context.Approvers.FirstOrDefaultAsync(x => x.Id == command.Id , cancellationToken);

                if (approver == null)
                {
                    return Result.Failure(ApproverError.ApproverNotExist());
                }

                approver.IsActive = !approver.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new UpdateApproverStatusResult
                {
                    Id = approver.Id,
                    Status = approver.IsActive

                };

                return Result.Success(results);
            }
        }
    }
}
