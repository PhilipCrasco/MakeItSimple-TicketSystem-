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
            public int ? TicketGeneratorId { get; set; }
            public string Re_Ticket_Remarks { get; set; }
            public Guid? Requestor_By { get; set; }
            public string Role { get; set; }
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

                var requestGeneratorIdInTransfer = await _context.ReTicketConcerns.FirstOrDefaultAsync(x => x.TicketGeneratorId == command.TicketGeneratorId, cancellationToken);
                var requestReTicketList = await _context.ReTicketConcerns.Where(x => x.TicketGeneratorId == command.TicketGeneratorId).ToListAsync();
                if (requestGeneratorIdInTransfer == null)
                {
                    return Result.Failure(ReTicketConcernError.TicketIdNotExist());
                }

                var validateApprover = await _context.ApproverTicketings.FirstOrDefaultAsync(x => x.TicketGeneratorId == requestGeneratorIdInTransfer.TicketGeneratorId
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
                            reTicketConcern.RejectRemarks = null;
                            reTicketConcern.RejectReTicketAt = null;
                            reTicketConcern.RejectReTicketBy = null;
                            reTicketConcern.IsRejectReTicket = false;
                            reTicketConcern.Remarks = command.Remarks;
                            reTicketHistoryList.Add(reTicketConcern);
                        }

                        transferUpdateList.Add(HasChange);
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
                            TicketGeneratorId = requestGeneratorIdInTransfer.TicketGeneratorId,
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
                                .Where(x => x.TicketGeneratorId == requestGeneratorIdInTransfer.Id && x.IsRejectReTicket == true
                                && x.UserId == command.Added_By).ToListAsync();

                            foreach (var reject in rejectTicketConcern)
                            {
                                reject.IsRejectReTicket = false;

                                reject.RejectRemarks = null;
                                reject.RejectReTicketBy = null;
                                reject.RejectReTicketAt = null;

                                if (reject.IsRejectReTicket is true)
                                {
                                    reject.Remarks = command.Remarks;
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

                if (reTicketHistoryList.Count(x => x.IsRejectReTicket is true) > 0)
                {

                    var addTicketHistory = new TicketHistory
                    {
                        TicketGeneratorId = reTicketHistoryList.First().TicketGeneratorId,
                        RequestorBy = command.Requestor_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.CloseTicket,
                        Status = TicketingConString.RequestUpdate
                    };

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
