using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate
{
    public class ApproveReDateTicket
    {
        public class ApproveReDateTicketCommand : IRequest<Result>
        {
            public Guid? ReDate_By { get; set; }
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Transacted_By { get; set; }
            public int TicketReDateId { get; set; }
        }

        public class Handler : IRequestHandler<ApproveReDateTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApproveReDateTicketCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await _context.Users
                   .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var allUserList = await _context.UserRoles.ToListAsync();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                var reDateExist = await _context.TicketReDates
                    .FirstOrDefaultAsync(x => x.Id == command.TicketReDateId, cancellationToken);

                if (reDateExist is null)
                {
                    return Result.Failure(ReDateError.ReDateIdNotExist());
                }

                var reDateRequestTicketId = await _context.ApproverTicketings
                    .Where(x => x.TicketReDateId == reDateExist.Id && x.IsApprove == null)
                    .ToListAsync();

                var selectReDateRequestId = reDateRequestTicketId
                    .FirstOrDefault(x => x.ApproverLevel == reDateRequestTicketId.Min(x => x.ApproverLevel));

                if (selectReDateRequestId is not null)
                {
                    if (!approverPermissionList.Any(x => x.Contains(command.Role) || reDateExist.TicketApprover != command.Users))
                    {
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                    }

                    selectReDateRequestId.IsApprove = true;

                    var userApprovalId = await _context.ApproverTicketings
                        .Where(x => x.TicketReDateId == selectReDateRequestId.TicketReDateId)
                        .ToListAsync();

                    var validateUserApprover = userApprovalId
                        .FirstOrDefault(x => x.ApproverLevel == selectReDateRequestId.ApproverLevel + 1);

                    if (validateUserApprover != null)
                    {
                        reDateExist.TicketApprover = validateUserApprover.UserId;

                    }
                    else
                    {
                        reDateExist.TicketApprover = null;
                        reDateExist.IsReDate = true;
                        reDateExist.ReDateBy = command.ReDate_By;
                        reDateExist.ReDateAt = DateTime.Now;

                        var ticketConcernExist = await _context.TicketConcerns
                            .FirstOrDefaultAsync(x => x.Id == reDateExist.TicketConcernId);


                        ticketConcernExist.IsReDate = true;
                        ticketConcernExist.ReDateAt = DateTime.Now;
                        ticketConcernExist.ReDateBy = reDateExist.UserId;

                        ticketConcernExist.Remarks = reDateExist.ReDateRemarks;
                        ticketConcernExist.StartDate = reDateExist.StartDate;
                        ticketConcernExist.TargetDate = reDateExist.TargetDate;

                        var addTicketHistory = new TicketHistory
                        {
                            TicketConcernId = reDateExist.TicketConcernId,
                            TransactedBy = command.Transacted_By,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.ReDate,
                            Status = $"{TicketingConString.ReDateApprove} {userDetails.Fullname}"
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                    }

                }
                else
                {
                    return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                }
                await  _context.SaveChangesAsync(cancellationToken);
                return Result.Success();    

            }
        }
    }
}
