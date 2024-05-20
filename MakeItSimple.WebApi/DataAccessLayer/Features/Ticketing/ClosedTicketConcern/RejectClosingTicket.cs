using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class RejectClosingTicket
    {
        public class RejectClosingTicketCommand : IRequest<Result>
        {
            public Guid? RejectClosed_By { get; set; }
            public Guid? Requestor_By { get; set; }
            public Guid? Approver_By { get; set; }
            //public string Role { get; set; }
            public string Reject_Remarks { get; set; }

            public ICollection<RejectClosedTicketConcern> RejectClosedTicketConcerns { get; set; }
            public class RejectClosedTicketConcern
            {
                public int TicketTransactionId { get; set; }

            }
        }

        public class Handler : IRequestHandler<RejectClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectClosingTicketCommand command, CancellationToken cancellationToken)
            {



                foreach (var close in command.RejectClosedTicketConcerns)
                {
                    var requestTransactionExist = await _context.TicketTransactions
                        .FirstOrDefaultAsync(x => x.Id == close.TicketTransactionId, cancellationToken);

                    if (requestTransactionExist == null)
                    {
                        return Result.Failure(ClosingTicketError.TicketIdNotExist());
                    }

                    var closedList = await _context.ClosingTickets
                        .Where(x => x.TicketTransactionId == requestTransactionExist.Id)
                        .ToListAsync();

                    var approverUserList = await _context.ApproverTicketings
                        .Where(x => x.TicketTransactionId == requestTransactionExist.Id)
                        .ToListAsync();

                    var approverLevelValidation = approverUserList
                        .FirstOrDefault(x => x.ApproverLevel == approverUserList.Min(x => x.ApproverLevel));

                    var ticketHistoryList = await _context.TicketHistories
                        .Where(x => x.TicketTransactionId == requestTransactionExist.Id)
                        .ToListAsync();

                    var ticketHistoryId = ticketHistoryList.FirstOrDefault(x => x.Id == ticketHistoryList.Max(x => x.Id));

                    foreach (var approverUserId in approverUserList)
                    {
                        approverUserId.IsApprove = null;
                    }

                    foreach (var perTicketId in closedList)
                    {
                        perTicketId.RejectClosedAt = DateTime.Now;
                        perTicketId.IsRejectClosed = true;
                        perTicketId.RejectClosedBy = command.RejectClosed_By;
                        perTicketId.TicketApprover = approverLevelValidation.UserId;
                        perTicketId.RejectRemarks = command.Reject_Remarks;
                    }

                    if (ticketHistoryId.Status != TicketingConString.RejectedBy)
                    {
                        var addTicketHistory = new TicketHistory
                        {
                            TicketTransactionId = requestTransactionExist.Id,
                            RequestorBy = closedList.First().AddedBy,
                            ApproverBy = command.Approver_By,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.CloseTicket,
                            Status = TicketingConString.RejectedBy
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


        }
    }
}
