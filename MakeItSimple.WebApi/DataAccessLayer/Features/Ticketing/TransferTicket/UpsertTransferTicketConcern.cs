using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class UpsertTransferTicketConcern
    {
        public class UpsertTransferTicketConcernCommand : IRequest<Result>
        {
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public Guid ? Transfer_By { get; set; }
            public List<AddTransferTicket> AddTransferTickets { get; set; }

            public class AddTransferTicket
            {
                public int ? TicketConcernId { get; set; }
                public int ? TransferTicketConcernId {  get; set; }
                public int SubUnitId { get; set; }
                public int ChannelId { get; set; }
                public Guid? UserId { get; set; }
                public string TransferRemarks { get; set; }
            }
        }

        public class Handler : IRequestHandler<UpsertTransferTicketConcernCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertTransferTicketConcernCommand command, CancellationToken cancellationToken)
            {
                foreach(var transferConcern in command.AddTransferTickets)
                {
                    switch(await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == transferConcern.SubUnitId, cancellationToken))
                    {
                        case null:
                            return Result.Failure(TransferTicketError.SubUnitNotExist());
                    }

                    switch (await _context.Channels.FirstOrDefaultAsync(x => x.Id == transferConcern.ChannelId, cancellationToken))
                    {
                        case null:
                            return Result.Failure(TransferTicketError.ChannelNotExist());
                    }

                    switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == transferConcern.UserId, cancellationToken))
                    {
                        case null:
                            return Result.Failure(TransferTicketError.UserNotExist());
                    }

                    if(command.AddTransferTickets.Count(x => x.TicketConcernId == transferConcern.TicketConcernId ) > 1)
                    {
                        return Result.Failure(TransferTicketError.DuplicateTransferTicket());
                    }

                    var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == transferConcern.TicketConcernId, cancellationToken);
                    var transferTicketConcern = await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.Id == transferConcern.TransferTicketConcernId, cancellationToken);
                    if (ticketConcern != null && transferTicketConcern == null)
                    {

                         var transferTicketAlreadyExist = await _context.TransferTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == transferConcern.TicketConcernId
                         && x.IsActive == true, cancellationToken);

                        if (transferTicketAlreadyExist != null)
                        {
                            return Result.Failure(TransferTicketError.TransferTicketAlreadyExist());  
                        }

                        var addTransferTicket = new TransferTicketConcern
                        {
                            TicketConcernId = ticketConcern.Id,
                           
                            SubUnitId = transferConcern.SubUnitId,
                            ChannelId = transferConcern.ChannelId,
                            UserId = transferConcern.UserId,
                            ConcernDetails = ticketConcern.ConcernDetails,
                            CategoryId = ticketConcern.CategoryId,
                            SubCategoryId = ticketConcern.SubCategoryId,
                            TransferRemarks = transferConcern.TransferRemarks,
                            AddedBy = command.Added_By,
                            StartDate = ticketConcern.StartDate,
                            TargetDate = ticketConcern.TargetDate,
                            TransferBy = command.Transfer_By,
                            TransferAt = DateTime.Now,
                            IsTransfer = false,

                        };

                        await _context.TransferTicketConcerns.AddAsync(addTransferTicket, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);

                    }
                    else if (ticketConcern == null && transferTicketConcern != null)
                    {
                        if (command.AddTransferTickets.Count(x => x.TransferTicketConcernId == transferConcern.TransferTicketConcernId) > 1)
                        {
                            return Result.Failure(TransferTicketError.DuplicateTransferTicket());
                        }


                        bool hasChanged = false;

                        if (transferTicketConcern.SubUnitId != transferConcern.SubUnitId)
                        {
                            transferTicketConcern.SubUnitId = transferConcern.SubUnitId;
                            hasChanged = true;
                        }
                        if (transferTicketConcern.ChannelId != transferConcern.ChannelId)
                        {
                            transferTicketConcern.ChannelId = transferConcern.ChannelId;
                            hasChanged = true;
                        }
                        if (transferTicketConcern.UserId != transferConcern.UserId)
                        {
                            transferTicketConcern.UserId = transferConcern.UserId;
                            hasChanged = true;
                        }

                        if(hasChanged)
                        {
                            transferTicketConcern.ModifiedBy = ticketConcern.ModifiedBy;
                            transferTicketConcern.UpdatedAt = DateTime.Now;
                            transferTicketConcern.IsActive = true;
                        }
                        
                    }
                    else
                    {
                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }


                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Success");
            }
        }

    }
}
