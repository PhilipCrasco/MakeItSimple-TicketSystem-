using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.UpsertTransferTicket.UpsertTransferTicketCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class UpsertTransferTicket
    {

        public record UpsertTransferTicketCommand : IRequest<Result>
        {  
            public Guid? Added_By { get; set; }
            public Guid? Requestor_By { get; set; }
            public Guid? Transfer_By { get; set; }
            public Guid? Modified_By { get; set; }
            public int ChannelId { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
            public string TransferRemarks { get; set; }
            public int ? RequestGeneratorId {  get; set; }
            public List<UpsertTransferTicketConcern> UpsertTransferTicketConcerns { get; set; }

            public class UpsertTransferTicketConcern
            {
                public int? TicketConcernId { get; set; }
                public int? TransferTicketConcernId { get; set; }
            }
        }


        public class Handler : IRequestHandler<UpsertTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var transferUpdateList = new List<bool>(); 
                var transferNewList = new List<TransferTicketConcern>();

                var requestGeneratorIdInTransfer = await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.RequestGeneratorId == command.RequestGeneratorId, cancellationToken);
                var requestTransferList = await _context.TransferTicketConcerns.Where(x => x.RequestGeneratorId == command.RequestGeneratorId).ToListAsync();
                if (requestGeneratorIdInTransfer == null)
                {
                    return Result.Failure(TransferTicketError.TicketIdNotExist());
                }

                var validateApprover = await _context.ApproverTicketings.FirstOrDefaultAsync(x => x.RequestGeneratorId == requestGeneratorIdInTransfer.RequestGeneratorId
                && x.IsApprove != null && x.ApproverLevel == 1, cancellationToken);

                if ((validateApprover is not null) || (requestGeneratorIdInTransfer.IsTransfer == true && command.Role != TicketingConString.Admin))
                {
                    return Result.Failure(TransferTicketError.TransferConcernUnable());
                }



                switch (await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken))
                {
                    case null:
                        return Result.Failure(TransferTicketError.ChannelNotExist());
                }

                switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken))
                {
                    case null:
                        return Result.Failure(TransferTicketError.UserNotExist());
                }

                foreach(var transfer in command.UpsertTransferTicketConcerns)
                {
                    if (command.UpsertTransferTicketConcerns.Count(x => x.TicketConcernId == transfer.TicketConcernId) > 1)
                    {
                        return Result.Failure(TransferTicketError.DuplicateConcernTicket());
                    }
                    else if (command.UpsertTransferTicketConcerns.Count(x => x.TransferTicketConcernId == transfer.TransferTicketConcernId) > 1)
                    {
                        return Result.Failure(TransferTicketError.DuplicateTransferTicket());
                    }

                    var ticketConcern  = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == transfer.TicketConcernId, cancellationToken);
                    if (ticketConcern == null)
                    {
                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }

                    if (ticketConcern.UserId == command.UserId)
                    {
                        return Result.Failure(TransferTicketError.InvalidTransferTicket());
                    }

                    var transferTicket = await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.Id == transfer.TransferTicketConcernId, cancellationToken);
                    if(ticketConcern != null && transferTicket != null)
                    {

                        var ticketConcernAlreadyExist = await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == transfer.TicketConcernId && transferTicket.TicketConcernId != transfer.TicketConcernId && x.IsTransfer == false, cancellationToken);
                        if (ticketConcernAlreadyExist != null)
                        {
                            return Result.Failure(TransferTicketError.TransferTicketAlreadyExist());
                        }

                        bool HasChange = false;

                        if (transferTicket.ChannelId != command.ChannelId)
                        {
                            transferTicket.ChannelId = command.ChannelId;
                            HasChange = true;
                        }

                        if (transferTicket.UserId != command.UserId)
                        {
                            transferTicket.UserId = command.UserId;
                            HasChange = true;
                        }
                        if (transferTicket.TransferRemarks != command.TransferRemarks)
                        {
                            transferTicket.TransferRemarks = command.TransferRemarks;
                            HasChange = true;
                        }

                        if(HasChange is true)
                        {
                            transferTicket.ModifiedBy = command.Modified_By;
                            transferTicket.UpdatedAt = DateTime.Now;
                            transferTicket.IsTransfer = false;
   
                        }

                        transferUpdateList.Add(HasChange);
                        

                    }
                    else if(ticketConcern != null && transferTicket is null)
                    {
                        var ticketConcernAlreadyExist = await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == transfer.TicketConcernId && x.IsTransfer == false, cancellationToken);
                        if (ticketConcernAlreadyExist != null)
                        {
                            return Result.Failure(TransferTicketError.TransferTicketAlreadyExist());
                        }

                        var addTransferTicket = new TransferTicketConcern
                        {
                            RequestGeneratorId = requestGeneratorIdInTransfer.RequestGeneratorId,
                            TicketConcernId = ticketConcern.Id,
                            ChannelId = command.ChannelId,
                            UserId = command.UserId,
                            ConcernDetails = ticketConcern.ConcernDetails,
                            //CategoryId = ticketConcern.CategoryId,
                            //SubCategoryId = ticketConcern.SubCategoryId,
                            TransferRemarks = command.TransferRemarks,
                            AddedBy = command.Added_By,
                            StartDate = ticketConcern.StartDate,
                            TargetDate = ticketConcern.TargetDate,
                            IsTransfer = false,
                            TicketApprover = requestGeneratorIdInTransfer.TicketApprover

                        };

                        ticketConcern.IsTransfer = false;
                        await _context.TransferTicketConcerns.AddAsync(addTransferTicket, cancellationToken);
                        transferNewList.Add(addTransferTicket);
                    }
                    else
                    {
                        return Result.Failure(TransferTicketError.TicketIdNotExist());
                    }

                }

                if ((requestTransferList.First().IsRejectTransfer == true && transferUpdateList.First() == true) 
                    || (transferNewList.Count > 0 && requestTransferList.First().IsRejectTransfer == true))
                {

                    var addTicketHistory = new TicketHistory
                    {
                        RequestGeneratorId = requestGeneratorIdInTransfer.RequestGeneratorId,
                        RequestorBy = command.Requestor_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Transfer,
                        Status = TicketingConString.RequestUpdate,

                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                    foreach (var ticketHistory in requestTransferList)
                    {
                        ticketHistory.IsRejectTransfer = false;
                        ticketHistory.RejectTransferAt = null;
                        ticketHistory.RejectTransferBy = null;
                    }


                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
