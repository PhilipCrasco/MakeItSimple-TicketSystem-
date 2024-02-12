using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class RejectTransferTicket
    {

        public class RejectTransferTicketCommand : IRequest<Result>
        { 
            public Guid ? RejectTransfer_By { get; set; }
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


                    foreach(var approverUserId in approverUserList)
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


                }
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
