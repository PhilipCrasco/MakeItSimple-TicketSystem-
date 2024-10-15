using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.ApprovalTransfer
{
    public partial class ApprovedTransferTicket
    {

        public class Handler : IRequestHandler<ApprovedTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);


                var transferTicketExist = await _context.TransferTicketConcerns
                    .Include(x => x.TicketConcern)
                    .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist is null)
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());

                var transferApprover = await _context.ApproverTicketings
                    .Where(x => x.TransferTicketConcernId == transferTicketExist.Id && x.IsApprove == null)
                    .FirstOrDefaultAsync();

                if (transferApprover is not null)
                {
                    
                    if (transferTicketExist.TicketApprover != command.Users)
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                    
                    transferApprover.IsApprove = true;
                    await UpdateTransferTicket(transferTicketExist,userDetails,command,cancellationToken);

                }
                else
                {
                    return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


            private async Task UpdateTicketHistory(TransferTicketConcern transferTicketConcern, User user, ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var ticketHistoryApproval = await _context.TicketHistories
                .Where(x => x.TicketConcernId == transferTicketConcern.TicketConcernId
                 && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                .FirstOrDefaultAsync(x => x.Approver_Level != null);

                ticketHistoryApproval.TransactedBy = command.Transacted_By;
                ticketHistoryApproval.TransactionDate = DateTime.Now;
                ticketHistoryApproval.Request = TicketingConString.Approve;
                ticketHistoryApproval.Status = $"{TicketingConString.TransferApprove} {user.Fullname}";
                ticketHistoryApproval.IsApprove = true;

            }

            private async Task UpdateTransferTicket(TransferTicketConcern transferTicketConcern,User user ,ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                transferTicketConcern.TicketApprover = null;

                transferTicketConcern.IsTransfer = true;
                transferTicketConcern.TransferBy = transferTicketConcern.TicketConcern.UserId;
                transferTicketConcern.TransferAt = DateTime.Now;

                var ticketConcernExist = await _context.TicketConcerns
                    .Include(x => x.RequestorByUser)
                    .FirstOrDefaultAsync(x => x.Id == transferTicketConcern.TicketConcernId);

                ticketConcernExist.IsTransfer = null;
                ticketConcernExist.Remarks = transferTicketConcern.TransferRemarks;
                ticketConcernExist.UserId = transferTicketConcern.TransferTo;
                ticketConcernExist.TargetDate = command.Target_Date;

                await CreateTicketHistory(transferTicketConcern, ticketConcernExist, user, command, cancellationToken);

            }

            private async Task CreateTicketHistory(TransferTicketConcern transferTicketConcern,TicketConcern ticketConcern,User user, ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var addNewTransferTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket concern number {ticketConcern.Id} has transfer",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.TransferBy.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = ticketConcern.Id ,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTransferTransactionNotification);

            }
        }
    }
}
