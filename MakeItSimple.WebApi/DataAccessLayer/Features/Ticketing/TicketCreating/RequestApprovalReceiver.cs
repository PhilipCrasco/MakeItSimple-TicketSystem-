﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class RequestApprovalReceiver
    {

        public class RequestApprovalReceiverCommand : IRequest<Result>
        {
            public Guid? Approved_By { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
            public int? TicketConcernId { get; set; }

            public string Modules { get; set; } 
            
        }


        public class Handler : IRequestHandler<RequestApprovalReceiverCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RequestApprovalReceiverCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var userDetails = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken);

                var allUserList = await _context.UserRoles.ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var ticketConcernExist = await _context.TicketConcerns
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId
                    && x.IsApprove != true, cancellationToken);

                if (ticketConcernExist == null)
                {
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());
                }

                var businessUnitList = await _context.BusinessUnits
                .FirstOrDefaultAsync(x => x.Id == ticketConcernExist.RequestorByUser.BusinessUnitId);

                var receiverList = await _context.Receivers
                .FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                if (receiverList == null)
                {
                    return Result.Failure(TicketRequestError.UnAuthorizedReceiver());
                }

                if (receiverList.UserId == command.UserId && receiverPermissionList.Contains(command.Role))
                {
                    if (ticketConcernExist.TargetDate < dateToday)
                    {
                        return Result.Failure(TicketRequestError.DateTimeInvalid());
                    }

                    ticketConcernExist.IsApprove = true;
                    ticketConcernExist.ApprovedBy = command.Approved_By;
                    ticketConcernExist.ApprovedAt = DateTime.Now;
                    ticketConcernExist.IsReject = false;
                    ticketConcernExist.ConcernStatus = TicketingConString.CurrentlyFixing;

                    if (ticketConcernExist.RequestConcernId != null)
                    {
                        var requestConcernList = await _context.RequestConcerns
                        .Where(x => x.Id == ticketConcernExist.RequestConcernId)
                        .FirstOrDefaultAsync();

                        requestConcernList.ConcernStatus = TicketingConString.CurrentlyFixing;

                    }

                    var addTicketHistory = new TicketHistory
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        TransactedBy = command.UserId,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.ConcernAssign,
                        Status = $"{TicketingConString.RequestAssign} {userDetails.Fullname}"
                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                }



                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
