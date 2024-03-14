﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class UpsertClosingTicket
    {
        public class UpsertClosingTicketCommand : IRequest<Result>
        {
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public int? TicketGeneratorId { get; set; }
            public Guid? Requestor_By { get; set; }
            public string Role { get; set; }

            public List<UpsertClosingTicketConcern> UpsertClosingTicketConcerns {  get; set; }
            public class UpsertClosingTicketConcern
            {
                public int TicketConcernId { get; set; }
                public int? ClosingTicketId { get; set; }
            }

            public class Handler : IRequestHandler<UpsertClosingTicketCommand, Result>
            {
                private readonly MisDbContext _context;

                public Handler(MisDbContext context)
                {
                    _context = context;
                }

                public async Task<Result> Handle(UpsertClosingTicketCommand command, CancellationToken cancellationToken)
                {
                    var closeUpdateList = new List<bool>();
                    var closeNewList = new List<ClosingTicket>();

                    var requestGeneratorIdInTransfer = await _context.ClosingTickets.FirstOrDefaultAsync(x => x.TicketGeneratorId == command.TicketGeneratorId, cancellationToken);
                    var requestClosingList = await _context.ClosingTickets.Where(x => x.TicketGeneratorId == command.TicketGeneratorId).ToListAsync();
                    if (requestGeneratorIdInTransfer == null)
                    {
                        return Result.Failure(ClosingTicketError.TicketIdNotExist());
                    }

                    var validateApprover = await _context.ApproverTicketings.FirstOrDefaultAsync(x => x.TicketGeneratorId == requestGeneratorIdInTransfer.TicketGeneratorId
                    && x.IsApprove != null && x.ApproverLevel == 1, cancellationToken);

                    if ((validateApprover is not null) || (requestGeneratorIdInTransfer.IsClosing == true && command.Role != TicketingConString.Receiver))
                    {
                        return Result.Failure(ClosingTicketError.ClosingTicketConcernUnable());
                    }

                    foreach( var close in command.UpsertClosingTicketConcerns )
                    {
                        var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == close.TicketConcernId, cancellationToken);
                        if (ticketConcern == null)
                        {

                            return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                        }

                        if (command.UpsertClosingTicketConcerns.Count(x => x.TicketConcernId == close.TicketConcernId) > 1)
                        {
                            return Result.Failure(TransferTicketError.DuplicateConcernTicket());
                        }
                        else if (command.UpsertClosingTicketConcerns.Count(x => x.ClosingTicketId == close.ClosingTicketId) > 1)
                        {
                            return Result.Failure(TransferTicketError.DuplicateTransferTicket());
                        }

                        var closingTicketConcern = await _context.ClosingTickets.FirstOrDefaultAsync(x => x.Id == close.ClosingTicketId, cancellationToken);
                        if(ticketConcern != null && closingTicketConcern != null)
                        {
                            var ticketConcernAlreadyExist = await _context.ClosingTickets.FirstOrDefaultAsync(x => x.TicketConcernId == close.TicketConcernId
                            && close.TicketConcernId != close.TicketConcernId && x.IsClosing == false, cancellationToken);
                            if (ticketConcernAlreadyExist != null)
                            {
                                return Result.Failure(ClosingTicketError.ClosingTicketIdAlreadyExist());
                            }

                            bool HasChange = false;

                            if(closingTicketConcern.TicketConcernId != close.TicketConcernId)
                            {
                                closingTicketConcern.TicketConcernId = close.TicketConcernId;

                                var ticketConcernList = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == closingTicketConcern.TicketConcernId, cancellationToken);

                                closingTicketConcern.StartDate = ticketConcernList.StartDate;
                                closingTicketConcern.TargetDate = ticketConcernList.TargetDate;
                               
                                HasChange = true;
                            }
                             
                            if (HasChange is true)
                            {
                                closingTicketConcern.ModifiedBy = command.Modified_By;
                                closingTicketConcern.UpdatedAt = DateTime.Now;
                                closingTicketConcern.IsClosing = false;
                                closingTicketConcern.ClosingAt = null;
                                closingTicketConcern.ClosedBy = null;
                            }

                            closeUpdateList.Add(HasChange);
                        }
                        else if (ticketConcern != null && closingTicketConcern is null)
                        {
                            var closingAlreadyExist = await _context.ClosingTickets.FirstOrDefaultAsync(x => x.TicketConcernId == close.TicketConcernId
                            && x.IsClosing == false, cancellationToken);

                            if (closingAlreadyExist != null)
                            {
                                return Result.Failure(ClosingTicketError.TicketConcernIdAlreadyExist());
                            }

                            var addClosingTicket = new ClosingTicket
                            {
                                TicketGeneratorId = requestGeneratorIdInTransfer.TicketGeneratorId,
                                TicketConcernId = ticketConcern.Id,
                                UserId = ticketConcern.UserId,
                                ConcernDetails = ticketConcern.ConcernDetails,
                                AddedBy = command.Added_By,
                                StartDate = ticketConcern.StartDate,
                                TargetDate = ticketConcern.TargetDate,
                                IsClosing = false,
                                TicketApprover = requestGeneratorIdInTransfer.TicketApprover

                            };

                            ticketConcern.IsClosedApprove = false;
                            await _context.ClosingTickets.AddAsync(addClosingTicket, cancellationToken);

                        }
                        else
                        {
                            return Result.Failure(ClosingTicketError.TicketIdNotExist());
                        }

                    }


                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success();
                }
            }
        }
    }
}