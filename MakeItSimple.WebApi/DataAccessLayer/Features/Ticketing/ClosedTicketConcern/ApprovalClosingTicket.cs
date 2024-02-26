using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class ApprovalClosingTicket
    {
        public class ApproveClosingTicketCommand : IRequest<Result>
        {
            public Guid? Closed_By { get; set; }public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Requestor_By { get; set; }
            public Guid? Approver_By { get; set; }
            public List<ApproveClosingRequest> ApproveClosingRequests { get; set; }

           public class ApproveClosingRequest
            {
                public int? RequestGeneratorId { get; set; }
            }
        }

        public class Handler : IRequestHandler<ApproveClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApproveClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                foreach(var close in command.ApproveClosingRequests)
                {
                    var requestTicketExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == close.RequestGeneratorId, cancellationToken);
                    if (requestTicketExist == null)
                    {
                        return Result.Failure(ClosingTicketError.TicketIdNotExist());
                    }

                    var ticketHistoryList = await _context.TicketHistories.Where(x => x.RequestGeneratorId == x.RequestGeneratorId).ToListAsync();
                    var ticketHistoryId = ticketHistoryList.FirstOrDefault(x => x.Id == ticketHistoryList.Max(x => x.Id));

                    var userClosedTicket = await _context.ClosingTickets.Where(x => x.RequestGeneratorId == requestTicketExist.Id).ToListAsync();
                    var closedRequestId = await _context.ApproverTicketings
                        .Where(x => x.RequestGeneratorId == requestTicketExist.Id && x.IsApprove == null).ToListAsync();

                    var selectClosedRequestId = closedRequestId.FirstOrDefault(x => x.ApproverLevel == closedRequestId.Min(x => x.ApproverLevel));

                    if (selectClosedRequestId != null)
                    {
                        selectClosedRequestId.IsApprove = true;

                        if (userClosedTicket.First().TicketApprover != command.Users)
                        {
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                        }

                        var userApprovalId = await _context.ApproverTicketings.Where(x => x.RequestGeneratorId == selectClosedRequestId.RequestGeneratorId).ToListAsync();

                        foreach( var concernTicket in userClosedTicket)
                        {

                            var validateUserApprover = userApprovalId.FirstOrDefault(x => x.ApproverLevel == selectClosedRequestId.ApproverLevel + 1);

                            if (validateUserApprover != null)
                            {
                                concernTicket.TicketApprover = validateUserApprover.UserId;
                            }
                            else
                            {
                                concernTicket.TicketApprover = null;
                            }
                        }

                        var approverLevel = selectClosedRequestId.ApproverLevel == 1 ? $"{selectClosedRequestId.ApproverLevel}st" 
                            : selectClosedRequestId.ApproverLevel == 2 ? $"{selectClosedRequestId.ApproverLevel}nd" 
                            : selectClosedRequestId.ApproverLevel == 3 ? $"{selectClosedRequestId.ApproverLevel}rd"
                            : $"{selectClosedRequestId.ApproverLevel}th";

                        var addTicketHistory = new TicketHistory
                        {
                            RequestGeneratorId = requestTicketExist.Id,
                            ApproverBy = command.Approver_By,
                            RequestorBy = userClosedTicket.First().AddedBy,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.CloseTicket,
                            Status = $"{TicketingConString.ApproveBy} {approverLevel} approver"
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);
                    }
                    else
                    {

                        if (TicketingConString.ApproverTransfer != command.Role)
                        {
                            return Result.Failure(ClosingTicketError.ApproverUnAuthorized());
                        }

                        foreach (var concernTicket in userClosedTicket)
                        {
                            concernTicket.IsClosing = true;
                            concernTicket.ClosingAt = DateTime.Now;
                            concernTicket.ClosedBy = command.Closed_By;

                            var concernTicketById = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == concernTicket.TicketConcernId, cancellationToken);

                            concernTicketById.IsClosedApprove = true;
                            concernTicketById.Closed_At = DateTime.Now;
                            concernTicketById.ClosedApproveBy = command.Closed_By;
                            concernTicketById.RequestGeneratorId = requestTicketExist.Id;
                            concernTicketById.Remarks = TicketingConString.CloseTicket;
                        }

                        if (ticketHistoryId.Status != TicketingConString.RecieverApproveBy)
                        {
                            var addTicketHistory = new TicketHistory
                            {
                                RequestGeneratorId = requestTicketExist.Id,
                                ApproverBy = command.Approver_By,
                                RequestorBy = userClosedTicket.First().AddedBy,
                                TransactionDate = DateTime.Now,
                                Request = TicketingConString.CloseTicket,
                                Status = TicketingConString.RecieverApproveBy
                            };

                            await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);
                        }


                    }
                 
                }



                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
               
            }
        }
    }
}
