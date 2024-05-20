using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class ApproveReTicket
    {
        public class ApproveReTicketCommand : IRequest<Result>
        {
            public Guid ? Re_Ticket_By { get; set; }
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Requestor_By { get; set; }
            public Guid? Approver_By { get; set; }
            public Guid ? Added_By { get; set; }
            public List<ApproveReTicketRequest> ApproveReTicketRequests { get; set; }
            public class ApproveReTicketRequest
            {
                public int ? TicketTransactionId { get; set; }
            }
        }

        public class Handler : IRequestHandler<ApproveReTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApproveReTicketCommand command, CancellationToken cancellationToken)
            {

                var dateToday = DateTime.Today;

                var allUserList = await _context.UserRoles.ToListAsync();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                foreach (var reTicket in command.ApproveReTicketRequests)
                {
                    var requestTicketExist = await _context.TicketTransactions
                        .FirstOrDefaultAsync(x => x.Id == reTicket.TicketTransactionId, cancellationToken);

                    if (requestTicketExist == null)
                    {
                        return Result.Failure(ReTicketConcernError.TicketIdNotExist());
                    }

                    var ticketHistoryList = await _context.TicketHistories
                        .Where(x => x.TicketTransactionId == x.TicketTransactionId)
                        .ToListAsync();

                    var ticketHistoryId = ticketHistoryList.FirstOrDefault(x => x.Id == ticketHistoryList.Max(x => x.Id));

                    var userReTicket = await _context.ReTicketConcerns
                        .Where(x => x.TicketTransactionId == requestTicketExist.Id)
                        .ToListAsync();

                    var reTicketRequestId = await _context.ApproverTicketings
                        .Where(x => x.TicketTransactionId == requestTicketExist.Id && x.IsApprove == null)
                        .ToListAsync();

                    var selectTransferRequestId = reTicketRequestId
                        .FirstOrDefault(x => x.ApproverLevel == reTicketRequestId.Min(x => x.ApproverLevel));

                    if (selectTransferRequestId != null)
                    {
                        selectTransferRequestId.IsApprove = true;

                        if (!approverPermissionList.Any(x => x.Contains(command.Role)))
                        {
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                        }

                        if (userReTicket.First().TicketApprover != command.Users)
                        {
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                        }

                        var userApprovalId = await _context.ApproverTicketings
                            .Where(x => x.TicketTransactionId == selectTransferRequestId.TicketTransactionId)
                            .ToListAsync();

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
                                concernTicket.IsReTicket = true;
                                concernTicket.ReTicketAt = DateTime.Now;
                                concernTicket.ReTicketBy = command.Re_Ticket_By;

                                var concernTicketById = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == concernTicket.TicketConcernId, cancellationToken);

                                concernTicketById.IsReTicket = true;
                                concernTicketById.ReticketAt = DateTime.Now;
                                concernTicketById.ReticketBy = command.Re_Ticket_By;
                                //concernTicketById.Remarks = TicketingConString.ReTicket;

                                var addNewTicketConcern = new TicketConcern
                                {
                                    RequestTransactionId = concernTicketById.RequestTransactionId,
                                    RequestorBy = concernTicketById.RequestorBy,
                                    ChannelId = concernTicketById.ChannelId,
                                    UserId = concernTicketById.UserId,
                                    ConcernDetails = concernTicketById.ConcernDetails,
                                    CategoryId = concernTicketById.CategoryId,
                                    SubCategoryId = concernTicketById.SubCategoryId,
                                    StartDate = concernTicket.StartDate,
                                    TargetDate = concernTicket.TargetDate,
                                    Remarks = concernTicketById.Remarks,
                                    IsApprove = true,
                                    RequestConcernId = concernTicketById.RequestConcernId,
                                    TicketType = concernTicketById.TicketType,
                                    ConcernStatus = concernTicketById.ConcernStatus,
                                    AddedBy = command.Added_By,
                                    
                                };

                                await  _context.TicketConcerns.AddAsync(addNewTicketConcern,cancellationToken);
                                await _context.SaveChangesAsync(cancellationToken);                                      

                                concernTicketById.Remarks = TicketingConString.ReTicket;
                            }

                        }

                        var approverLevel = selectTransferRequestId.ApproverLevel == 1 ? $"{selectTransferRequestId.ApproverLevel}st" 
                            : selectTransferRequestId.ApproverLevel == 2 ? $"{selectTransferRequestId.ApproverLevel}nd" 
                            : selectTransferRequestId.ApproverLevel == 3 ? $"{selectTransferRequestId.ApproverLevel}rd"
                            : $"{selectTransferRequestId.ApproverLevel}th";

                        var addTicketHistory = new TicketHistory
                        {
                            TicketTransactionId  = requestTicketExist.Id,
                            ApproverBy = command.Approver_By,
                            RequestorBy = userReTicket.First().AddedBy,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.ReTicket,
                            Status = $"{ TicketingConString.ApproveBy} {approverLevel} approver"
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                    }
                    else
                    {
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                    }


                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
