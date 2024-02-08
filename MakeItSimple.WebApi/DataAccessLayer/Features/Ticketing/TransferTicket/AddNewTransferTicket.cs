using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class AddNewTransferTicket
    {
        public class AddNewTransferTicketCommand : IRequest<Result>
        {
            public Guid ? Added_By { get; set; }
            public Guid ? Transfer_By { get; set; }
            public int SubUnitId { get; set; }
            public int ChannelId { get; set; }
            public Guid? UserId { get; set; }
            public string TransferRemarks { get; set; }

            public List<AddTransferTicket> AddTransferTickets { get; set; }

            public class AddTransferTicket
            {
                public int ? TicketConcernId { get; set; }
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
                var requestGeneratedId = new RequestGenerator { };

                await  _context.RequestGenerators.AddAsync(requestGeneratedId, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                switch (await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken))
                {
                    case null:
                        return Result.Failure(TransferTicketError.SubUnitNotExist());
                }

                var channelexist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);
                if (channelexist is null)
                {
                    return Result.Failure(TransferTicketError.ChannelNotExist());
                }
                
                switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken))
                {
                    case null:
                        return Result.Failure(TransferTicketError.UserNotExist());
                }

                foreach (var transferConcern in command.AddTransferTickets)
                {

                    if(command.AddTransferTickets.Count(x => x.TicketConcernId == transferConcern.TicketConcernId ) > 1)
                    {
                        return Result.Failure(TransferTicketError.DuplicateConcernTicket());
                    }

                    var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == transferConcern.TicketConcernId, cancellationToken);
                    if (ticketConcern != null )
                    {

                        var transferTicketAlreadyExist = await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == transferConcern.TicketConcernId
                          && x.IsActive == true, cancellationToken);

                        if (transferTicketAlreadyExist != null)
                        {
                            return Result.Failure(TransferTicketError.TransferTicketAlreadyExist());  
                        }


                        var getApproverUser = await _context.Approvers.Where(x => x.ChannelId == ticketConcern.ChannelId).ToListAsync();
                        
                        var getApproverUserId = getApproverUser.First(x => x.ApproverLevel == getApproverUser.Min(x => x.ApproverLevel));

                        var addTransferTicket = new TransferTicketConcern
                        {
                            RequestGeneratorId = requestGeneratedId.Id,
                            TicketConcernId = ticketConcern.Id,
                            DepartmentId = ticketConcern.DepartmentId,
                            SubUnitId = command.SubUnitId,
                            ChannelId = command.ChannelId,
                            UserId = command.UserId,
                            ConcernDetails = ticketConcern.ConcernDetails,
                            CategoryId = ticketConcern.CategoryId,
                            SubCategoryId = ticketConcern.SubCategoryId,
                            TransferRemarks = command.TransferRemarks,
                            AddedBy = command.Added_By,
                            StartDate = ticketConcern.StartDate,
                            TargetDate = ticketConcern.TargetDate,
                            //TransferBy = command.Transfer_By,
                            //TransferAt = DateTime.Now,
                            IsTransfer = false,
                            TicketApprover = getApproverUserId.UserId
                            
                        };


                        ticketConcern.IsTransfer = false;
                        transferList.Add(ticketConcern);

                        await _context.TransferTicketConcerns.AddAsync(addTransferTicket, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);

                    }
                    else
                    {
                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }


                }

       
                var getApprover = await _context.Approvers
                    .Where(x => x.ChannelId == transferList.First().ChannelId).ToListAsync();


                if (getApprover == null)
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                foreach (var approver in getApprover)
                {
                    var addNewApprover = new ApproverTicketing
                    {
                        RequestGeneratorId = requestGeneratedId.Id,
                        ChannelId = approver.ChannelId,
                        UserId = approver.UserId,
                        ApproverLevel = approver.ApproverLevel,
                        AddedBy = command.Added_By,
                        CreatedAt = DateTime.Now,
                        Status = "Transfer",

                    };

                    await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Result.Success();
            }
        }

    }
}
