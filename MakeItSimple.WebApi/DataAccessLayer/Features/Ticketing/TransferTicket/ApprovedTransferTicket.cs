using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class ApprovedTransferTicket
    {

        public class ApprovedTransferTicketCommand : IRequest<Result>
        {
            public Guid? Transfer_By { get; set; }
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Requestor_By { get; set; }
            public Guid? Approver_By { get; set; }
            public int TransferTicketId { get; set; }

        }

        public class Handler : IRequestHandler<ApprovedTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {


                var allUserList = await _context.UserRoles.ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                var transferTicketExist = await _context.TransferTicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist is null)
                {
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());
                }

                var transferRequestTicketId = await _context.ApproverTicketings
                    .Where(x => x.TransferTicketConcernId == transferTicketExist.Id && x.IsApprove == null)
                    .ToListAsync();

                var selectTransferRequestId = transferRequestTicketId
                    .FirstOrDefault(x => x.ApproverLevel == transferRequestTicketId.Min(x => x.ApproverLevel));

                if (selectTransferRequestId != null)
                {

                    selectTransferRequestId.IsApprove = true;

                    if (!approverPermissionList.Any(x => x.Contains(command.Role)))
                    {
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                    }

                    if (transferTicketExist.TicketApprover != command.Users)
                    {
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                    }

                    var userApprovalId = await _context.ApproverTicketings
                        .Where(x => x.TransferTicketConcernId == selectTransferRequestId.TransferTicketConcernId)
                        .ToListAsync();

                    var validateUserApprover = userApprovalId
                        .FirstOrDefault(x => x.ApproverLevel == selectTransferRequestId.ApproverLevel + 1);

                    if (validateUserApprover != null)
                    {
                        transferTicketExist.TicketApprover = validateUserApprover.UserId;
                    }
                    else
                    {
                        transferTicketExist.TicketApprover = null;

                        transferTicketExist.IsTransfer = true;
                        transferTicketExist.TransferBy = command.Transfer_By;
                        transferTicketExist.TransferAt = DateTime.Now;


                    }
                
                  
                    var approverLevel = selectTransferRequestId.ApproverLevel == 1 ? $"{selectTransferRequestId.ApproverLevel}st"
                        : selectTransferRequestId.ApproverLevel == 2 ? $"{selectTransferRequestId.ApproverLevel}nd"
                        : selectTransferRequestId.ApproverLevel == 3 ? $"{selectTransferRequestId.ApproverLevel}rd"
                        : $"{selectTransferRequestId.ApproverLevel}th";

                    //var addTicketHistory = new TicketHistory
                    //{
                    //    RequestTransactionId = requestTicketExist.Id,
                    //    ApproverBy = command.Approver_By,
                    //    RequestorBy = userTranferTicket.First().AddedBy,
                    //    TransactionDate = DateTime.Now,
                    //    Request = TicketingConString.Transfer,
                    //    Status = $"{TicketingConString.ApproveBy} {approverLevel} approver"
                    //};

                    //await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                }
                else
                {

                    return Result.Failure(TransferTicketError.ApproverUnAuthorized());


                    //if (ticketHistoryId.Status != TicketingConString.ReceiverApproveBy)
                    //{
                    //    var addTicketHistory = new TicketHistory
                    //    {
                    //        RequestGeneratorId = requestTicketExist.Id,
                    //        ApproverBy = command.Approver_By,
                    //        RequestorBy = userTranferTicket.First().AddedBy,
                    //        TransactionDate = DateTime.Now,
                    //        Request = TicketingConString.Transfer,
                    //        Status = TicketingConString.ReceiverApproveBy
                    //    };

                    //    await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);
                    //}
                }


                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
