using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class ApproveReTicket
    {
        public class ApproveReTicketCommand : IRequest<Result>
        {
            public Guid ? Re_Ticket_By { get; set; }
            public List<ApproveReTicketRequest> ApproveReTicketRequests { get; set; }
            public class ApproveReTicketRequest
            {
                public int ? RequestGeneratorId { get; set; }
            }
        }

        public class Handler : IRequestHandler<ApproveReTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async  Task<Result> Handle(ApproveReTicketCommand command, CancellationToken cancellationToken)
            {

                var dateToday = DateTime.Today;

                foreach(var reTicket in command.ApproveReTicketRequests)
                {
                    var requestTicketExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == reTicket.RequestGeneratorId, cancellationToken);
                    if (requestTicketExist == null)
                    {
                        return Result.Failure(ReTicketConcernError.TicketIdNotExist());
                    }

                    var userReTicket = await _context.ReTicketConcerns.Where(x => x.RequestGeneratorId == requestTicketExist.Id).ToListAsync();
                    var reTicketRequestId = await _context.ApproverTicketings
                        .Where(x => x.RequestGeneratorId == requestTicketExist.Id && x.IsApprove == null).ToListAsync();



                    var selectTransferRequestId = reTicketRequestId.FirstOrDefault(x => x.ApproverLevel == reTicketRequestId.Min(x => x.ApproverLevel));

                    if (selectTransferRequestId != null)
                    {
                        selectTransferRequestId.IsApprove = true;

                        var userApprovalId = await _context.ApproverTicketings.Where(x => x.RequestGeneratorId == selectTransferRequestId.RequestGeneratorId).ToListAsync();

                        foreach (var concernTicket in userReTicket)
                        {

                            if(concernTicket.TargetDate < dateToday)
                            {
                                return Result.Failure(ReTicketConcernError.DateTimeInvalid());
                            }

                            var validateUserApprover = userApprovalId.FirstOrDefault(x => x.ApproverLevel == selectTransferRequestId.ApproverLevel + 1);

                            if (validateUserApprover != null)
                            {
                                concernTicket.TicketApprover = validateUserApprover.UserId;
                            }
                            else
                            {
                                concernTicket.TicketApprover = null;
                            }

                        }

                    }
                    else
                    {
                        foreach(var concernTicket  in userReTicket)
                        {
                            concernTicket.IsReTicket = true;
                            concernTicket.ReTicketAt = DateTime.Now;
                            concernTicket.ReTicketBy = command.Re_Ticket_By;

                            var concernTicketById = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == concernTicket.TicketConcernId, cancellationToken);

                            //concernTicketById.IsApprove = false;
                            concernTicketById.IsReTicket = true;
                            concernTicketById.ReticketAt = DateTime.Now;
                            concernTicketById.ReticketBy = command.Re_Ticket_By;
                            concernTicketById.RequestGeneratorId = requestTicketExist.Id;
                            concernTicketById.StartDate = Convert.ToDateTime(concernTicket.StartDate);
                            concernTicketById.TargetDate = Convert.ToDateTime(concernTicket.TargetDate);
                            concernTicketById.Remarks = TicketingConString.ReTicket;
                        }
                    }


                }

                await _context.SaveChangesAsync();
                return Result.Success();
            }
        }
    }
}
