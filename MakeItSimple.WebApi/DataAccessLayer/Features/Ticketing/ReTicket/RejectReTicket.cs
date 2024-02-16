using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class RejectReTicket
    {
        public class RejectReTicketCommand : IRequest<Result>
        {
            public Guid? RejectReTicket_By { get; set; }
            public ICollection<RejectReTicketConcern> RejectReTicketConcerns { get; set; }
            public class RejectReTicketConcern
            {
                public int RequestGeneratorId { get; set; }
                public string Reject_Remarks { get; set; }
            }

        }

        public class Handler : IRequestHandler<RejectReTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectReTicketCommand command, CancellationToken cancellationToken)
            {
                foreach (var reTicket in command.RejectReTicketConcerns)
                {
                    var requestGeneratorExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == reTicket.RequestGeneratorId, cancellationToken);

                    if (requestGeneratorExist == null)
                    {
                        return Result.Failure(TransferTicketError.TicketIdNotExist());
                    }

                    var reTicketList = await _context.ReTicketConcerns.Where(x => x.RequestGeneratorId == requestGeneratorExist.Id).ToListAsync();

                    var approverUserList = await _context.ApproverTicketings.Where(x => x.RequestGeneratorId == requestGeneratorExist.Id).ToListAsync();

                    var approverLevelValidation = approverUserList.FirstOrDefault(x => x.ApproverLevel == approverUserList.Min(x => x.ApproverLevel));


                    foreach (var approverUserId in approverUserList)
                    {
                        approverUserId.IsApprove = null;
                    }


                    foreach (var perTicketId in reTicketList)
                    {
                        perTicketId.RejectReTicketAt = DateTime.Now;
                        perTicketId.IsRejectReTicket = true;
                        perTicketId.RejectReTicketBy = command.RejectReTicket_By;
                        perTicketId.TicketApprover = approverLevelValidation.UserId;
                        perTicketId.RejectRemarks = reTicket.Reject_Remarks;
                    }


                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
