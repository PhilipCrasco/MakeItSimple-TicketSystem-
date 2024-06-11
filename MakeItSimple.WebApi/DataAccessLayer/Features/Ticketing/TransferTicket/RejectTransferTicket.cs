using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class RejectTransferTicket
    {

        public class RejectTransferTicketCommand : IRequest<Result>
        { 
            public Guid ? RejectTransfer_By { get; set; }
            public Guid ? Requestor_By { get; set; }
            public Guid ? Approver_By { get; set; }
            public string Role { get; set; }
            public int TransferTicketId { get; set; }

            public string Reject_Remarks { get; set; }
        }

        public class Handler : IRequestHandler<RejectTransferTicketCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var transferTicketExist = await _context.TransferTicketConcerns.
                    FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist == null)
                {
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());
                }

                var userRoleList = await _context.UserRoles.ToListAsync();

                var approverPermission = userRoleList
                .Where(x => x.Permissions.Contains(TicketingConString.Approver))
                .Select(x => x.UserRoleName);

                if (!approverPermission.Any(x => x.Contains(command.Role)))
                {
                    return Result.Failure(TicketRequestError.UnAuthorizedReceiver());
                }

                var approverUserList = await _context.ApproverTicketings
                    .Where(x => x.TransferTicketConcernId == transferTicketExist.Id)              
                    .ToListAsync();

                if(!approverUserList.Any())
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                foreach (var approverUserId in approverUserList)
                {
                    approverUserId.IsApprove = null;
                }

                var approverLevelValidation = approverUserList
                    .FirstOrDefault(x => x.ApproverLevel == approverUserList.Min(x => x.ApproverLevel));

                transferTicketExist.RejectTransferAt = DateTime.Now;
                transferTicketExist.IsRejectTransfer = true;
                transferTicketExist.RejectTransferBy = command.RejectTransfer_By;
                transferTicketExist.TicketApprover = approverLevelValidation.UserId;


                //var addTicketHistory = new TicketHistory
                //{
                //    TicketConcernId = transferTicketExist.TicketConcernId,
                //    RequestorBy = transferTicketExist.AddedBy,
                //    ApproverBy = command.Approver_By,
                //    TransactionDate = DateTime.Now,
                //    Request = TicketingConString.Transfer,
                //    Status = TicketingConString.RejectedBy
                //};

                //await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
