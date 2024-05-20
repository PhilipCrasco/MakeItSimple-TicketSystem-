using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class UpsertReTicket
    {
        public class UpsertReTicketCommand : IRequest<Result>
        {
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public int ? TicketTransactionId { get; set; }
            public string Re_Ticket_Remarks { get; set; }
            public Guid? Requestor_By { get; set; }
            //public string Role { get; set; }
            public string Remarks { get; set; }
            public List<UpsertReTicketConsern> UpsertReTicketConserns {  get; set; }

            public class UpsertReTicketConsern
            {
                public int ? TicketConcernId { get; set; }
                public int ? ReTicketConcernId { get; set; }
                public DateTime? Start_Date { get; set; }
                public DateTime? Target_Date { get; set; }

            }

        }

        public class Handler : IRequestHandler<UpsertReTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertReTicketCommand command, CancellationToken cancellationToken)
            {

                var transferUpdateList = new List<bool>();
                var reTicketHistoryList = new List<ReTicketConcern>();
                var removeTicketConcern = new List<ReTicketConcern>();

                var allUserList = await _context.UserRoles.ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                var requestGeneratorIdInTransfer = await _context.ReTicketConcerns
                    .FirstOrDefaultAsync(x => x.TicketTransactionId == command.TicketTransactionId, cancellationToken);

                var requestReTicketList = await _context.ReTicketConcerns
                    .Where(x => x.TicketTransactionId == command.TicketTransactionId)
                    .ToListAsync();

                if (requestGeneratorIdInTransfer == null)
                {
                    return Result.Failure(ReTicketConcernError.TicketIdNotExist());
                }

                var validateApprover = await _context.ApproverTicketings
                    .FirstOrDefaultAsync(x => x.TicketTransactionId == requestGeneratorIdInTransfer.TicketTransactionId
                && x.IsApprove != null && x.ApproverLevel == 1, cancellationToken);

                if (validateApprover != null)
                {
                    return Result.Failure(ReTicketConcernError.ReTicketConcernUnable());
                }

                foreach (var reTicket in command.UpsertReTicketConserns)
                {
                    var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == reTicket.TicketConcernId, cancellationToken);
                    if (ticketConcern == null)
                    {
                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }

                    if (command.UpsertReTicketConserns.Count(x => x.TicketConcernId == reTicket.TicketConcernId) > 1)
                    {
                        return Result.Failure(TransferTicketError.DuplicateConcernTicket());
                    }
                    else if (command.UpsertReTicketConserns.Count(x => x.ReTicketConcernId == reTicket.ReTicketConcernId) > 1)
                    {
                        return Result.Failure(TransferTicketError.DuplicateTransferTicket());
                    }

                    var reTicketConcern = await _context.ReTicketConcerns.FirstOrDefaultAsync(x => x.Id == reTicket.ReTicketConcernId, cancellationToken);
                    if (ticketConcern != null && reTicketConcern != null)
                    {

                        var ticketConcernAlreadyExist = await _context.ReTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == reTicket.TicketConcernId 
                        && reTicket.TicketConcernId != reTicket.TicketConcernId && x.IsReTicket == false, cancellationToken);
                        if (ticketConcernAlreadyExist != null)
                        {
                            return Result.Failure(TransferTicketError.TransferTicketAlreadyExist());
                        }

                        bool HasChange = false;

                        if (reTicketConcern.ReTicketRemarks != command.Re_Ticket_Remarks)
                        {
                            reTicketConcern.ReTicketRemarks = command.Re_Ticket_Remarks;
                            HasChange = true;
                        }

                        if (reTicketConcern.StartDate != reTicket.Start_Date)
                        {
                            reTicketConcern.StartDate = reTicket.Start_Date;
                            HasChange = true;
                        }

                        if (reTicketConcern.TargetDate != reTicket.Target_Date)
                        {
                            reTicketConcern.TargetDate = reTicket.Target_Date;
                            HasChange = true;
                        }

                        if (HasChange is true)
                        {
                            reTicketConcern.ModifiedBy = command.Modified_By;
                            reTicketConcern.UpdatedAt = DateTime.Now;
                            reTicketConcern.IsReTicket = false;
                            reTicketConcern.ReTicketAt = null;
                            reTicketConcern.ReTicketBy = null;
                        }

                        if(reTicketConcern.IsRejectReTicket is true)
                        {

                            reTicketConcern.Remarks = command.Remarks;
                            reTicketHistoryList.Add(reTicketConcern);
                        }

                        transferUpdateList.Add(HasChange);
                        removeTicketConcern.Add(reTicketConcern);
                    }
                    else if (ticketConcern != null && reTicketConcern is null)
                    {
                        var reTicketAlreadyExist = await _context.ReTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == reTicketConcern.TicketConcernId 
                        && x.IsReTicket == false, cancellationToken);

                        if (reTicketAlreadyExist != null)
                        {
                            return Result.Failure(ReTicketConcernError.TicketConcernIdAlreadyExist());
                        }

                        var addReTicket = new ReTicketConcern
                        {
                            TicketTransactionId = requestGeneratorIdInTransfer.TicketTransactionId,
                            UserId = ticketConcern.UserId,
                            ChannelId = ticketConcern.ChannelId,
                            ConcernDetails = ticketConcern.ConcernDetails,
                            ReTicketRemarks = command.Re_Ticket_Remarks,
                            AddedBy = command.Added_By,
                            Category = ticketConcern.Category,
                            SubCategory = ticketConcern.SubCategory,
                            StartDate = reTicket.Start_Date,
                            TargetDate = reTicket.Target_Date,
                            IsReTicket = false,
                            TicketApprover = requestGeneratorIdInTransfer.TicketApprover

                        };

                        if (requestGeneratorIdInTransfer != null)
                        {
                            var rejectTicketConcern = await _context.ReTicketConcerns
                                .Where(x => x.TicketTransactionId == requestGeneratorIdInTransfer.Id && x.IsRejectReTicket == true
                                && x.UserId == command.Added_By).ToListAsync();

                            foreach (var reject in rejectTicketConcern)
                            {

                                reject.RejectRemarks = null;

                                if (reject.IsRejectReTicket is true)
                                {
                                    reTicketHistoryList.Add(addReTicket);
                                }

                            }
                        }

                        ticketConcern.IsReTicket = false;
                        await _context.ReTicketConcerns.AddAsync(addReTicket, cancellationToken);
                    }
                    else
                    {
                        return Result.Failure(TransferTicketError.TicketIdNotExist());
                    }

                }

                var selectRemoveConcern = removeTicketConcern.Select(x => x.Id);
                var selectRemoveGenerator = removeTicketConcern.Select(x => x.TicketTransactionId);

                var removeConcernList = await _context.ReTicketConcerns
                    .Where(x => !selectRemoveConcern.Contains(x.Id)
                    && selectRemoveGenerator.Contains(x.TicketTransactionId)
                    && x.IsActive == true)
                    .ToListAsync();

                if (removeTicketConcern.Count() > 0)
                {
                    foreach (var removeConcern in removeTicketConcern)
                    {
                        removeConcern.IsActive = false;
                    }
                }

                if (reTicketHistoryList.Count(x => x.IsRejectReTicket is true) > 0)
                {
                   var reTicketList = await _context.ReTicketConcerns
                            .Where(x => x.TicketTransactionId == reTicketHistoryList.First().TicketTransactionId 
                            && x.IsRejectReTicket == true && x.UserId == reTicketHistoryList.First().UserId).ToListAsync();

                        foreach(var reTicket in reTicketList)
                        {
                            reTicket.RejectRemarks = null;
                            reTicket.RejectReTicketBy = null;
                            reTicket.IsRejectReTicket = false;
                            reTicket.RejectReTicketAt = null;
                        }

                    var addTicketHistory = new TicketHistory
                    {
                        TicketTransactionId = reTicketHistoryList.First().TicketTransactionId,
                        RequestorBy = command.Requestor_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.ReTicket,
                        Status = TicketingConString.RequestUpdate
                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory,cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
