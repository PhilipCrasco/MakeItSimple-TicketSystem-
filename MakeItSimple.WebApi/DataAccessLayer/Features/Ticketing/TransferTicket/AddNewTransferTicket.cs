using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.AddNewTransferTicket.AddNewTransferTicketCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class AddNewTransferTicket
    {
        public class AddNewTransferTicketCommand : IRequest<Result>
        {
            public Guid ? Added_By { get; set; }
            public Guid ? Requestor_By { get; set; }
            public Guid ? Transfer_By { get; set; }
            public int ChannelId { get; set; }
            public string TransferRemarks { get; set; }
            public List<AddTransferTicket> AddTransferTickets { get; set; }

            public class AddTransferTicket
            {
                public int ? TicketConcernId { get; set; }
                public Guid? UserId { get; set; }
                public DateTime ? Start_Date { get; set; }
                public DateTime ? Target_Date { get; set; }
            }
        }

        public class Handler : IRequestHandler<AddNewTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var transferList = new List<TicketConcern>();
                var ticketGenerator = new TicketGenerator { IsActive = true };

                await  _context.TicketGenerators.AddAsync(ticketGenerator, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var channelexist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);
                if (channelexist is null)
                {
                    return Result.Failure(TransferTicketError.ChannelNotExist());
                }
               
                foreach (var transferConcern in command.AddTransferTickets)
                {

                    if(command.AddTransferTickets.Count(x => x.TicketConcernId == transferConcern.TicketConcernId ) > 1)
                    {
                        return Result.Failure(TransferTicketError.DuplicateConcernTicket());
                    }


                    var userExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == transferConcern.UserId, cancellationToken);

                    if(userExist is null)
                    {
                            return Result.Failure(TransferTicketError.UserNotExist());
                    }

                    var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == transferConcern.TicketConcernId, cancellationToken);
                    if (ticketConcern != null )
                    {

                        if(ticketConcern.UserId == transferConcern.UserId) 
                        {
                            return Result.Failure(TransferTicketError.InvalidTransferTicket());
                        }

                        var transferTicketAlreadyExist = await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == transferConcern.TicketConcernId
                          && x.IsActive == true && x.IsTransfer == false, cancellationToken);

                        if (transferTicketAlreadyExist != null)
                        {
                            return Result.Failure(TransferTicketError.TransferTicketAlreadyExist());  
                        }

                        var getApproverUser = await _context.Approvers.Include(x => x.User).Where(x => x.SubUnitId == userExist.SubUnitId).ToListAsync();
                        
                        var getApproverUserId = getApproverUser.First(x => x.ApproverLevel == getApproverUser.Min(x => x.ApproverLevel));

                        var addTransferTicket = new TransferTicketConcern
                        {
                            TicketGeneratorId = ticketGenerator.Id,
                            TicketConcernId = ticketConcern.Id,
                            ChannelId = command.ChannelId,
                            UserId = transferConcern.UserId,
                            ConcernDetails = ticketConcern.ConcernDetails,
                            TransferRemarks = command.TransferRemarks,
                            AddedBy = command.Added_By,
                            StartDate = ticketConcern.StartDate,
                            TargetDate = ticketConcern.TargetDate,
                            IsTransfer = false,
                            TicketApprover = getApproverUserId.UserId

                        };

                        ticketConcern.IsTransfer = false; 

                        var ticketConcernApproverExist = await _context.TransferTicketConcerns
                            .FirstOrDefaultAsync(x => x.TicketConcernId == transferConcern.TicketConcernId);
                        {

                        }

                        transferList.Add(ticketConcern);
                        await _context.TransferTicketConcerns.AddAsync(addTransferTicket, cancellationToken);

                        await _context.SaveChangesAsync(cancellationToken);

                    }
                    else
                    {
                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }

                }

                var getApprover = await _context.Approvers.Include(x => x.User)
                    .Where(x => x.SubUnitId == transferList.First().User.SubUnitId).ToListAsync();


                if (getApprover == null)
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                foreach (var approver in getApprover)
                {
                    var addNewApprover = new ApproverTicketing
                    {
                        TicketGeneratorId = ticketGenerator.Id,
                        SubUnitId = approver.SubUnitId,
                        UserId = approver.UserId,
                        ApproverLevel = approver.ApproverLevel,
                        AddedBy = command.Added_By,
                        CreatedAt = DateTime.Now,
                        Status = "Transfer",

                    };

                    await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);
                }

                var addTicketHistory = new TicketHistory
                {
                    
                   TicketGeneratorId = ticketGenerator.Id,
                   RequestorBy = command.Requestor_By,
                   TransactionDate = DateTime.Now,
                   Request = TicketingConString.Transfer,
                   Status = TicketingConString.RequestCreated

                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }

    }
}
 