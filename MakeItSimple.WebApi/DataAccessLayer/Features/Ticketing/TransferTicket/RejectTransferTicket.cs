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

            public ICollection<RejectTransferTicket> RejectTransferTickets { get; set; }
            public class RejectTransferTicket
            {
                public int RequestGeneratorId { get; set; }
                public string Reject_Remarks { get; set; }
            }
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
                foreach(var transferTicket in command.RejectTransferTickets)
                {
                    var requestGeneratorExist= await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == transferTicket.RequestGeneratorId, cancellationToken);

                    if (requestGeneratorExist == null) 
                    {
                        return Result.Failure(TransferTicketError.TicketIdNotExist());
                    }

                    var transferTicketList = await _context.TransferTicketConcerns.Where(x => x.RequestGeneratorId == requestGeneratorExist.Id).ToListAsync();

                    var approverUserList = await _context.ApproverTicketings.Where(x => x.RequestGeneratorId == requestGeneratorExist.Id).ToListAsync();

                    var approverLevelValidation =  approverUserList.FirstOrDefault(x => x.ApproverLevel == approverUserList.Min(x => x.ApproverLevel));

                    var ticketHistoryList = await _context.TicketHistories.Where(x => x.RequestGeneratorId == x.RequestGeneratorId).ToListAsync();
                    var ticketHistoryId = ticketHistoryList.FirstOrDefault(x => x.Id == ticketHistoryList.Max(x => x.Id));

                    foreach (var approverUserId in approverUserList)
                    {
                        approverUserId.IsApprove = null;
                    }


                    foreach (var perTicketId in transferTicketList)
                    {
                        perTicketId.RejectTransferAt = DateTime.Now;
                        perTicketId.IsRejectTransfer = true;
                        perTicketId.RejectTransferBy = command.RejectTransfer_By;
                        perTicketId.TicketApprover = approverLevelValidation.UserId;
                        perTicketId.RejectRemarks = transferTicket.Reject_Remarks;
                    }

                    if (ticketHistoryId.Status != TicketingConString.RejectedBy)
                    {
                        var addTicketHistory = new TicketHistory
                        {
                            RequestGeneratorId = requestGeneratorExist.Id,
                            RequestorBy = transferTicketList.First().AddedBy,
                            ApproverBy = command.Approver_By,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.Transfer,
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
