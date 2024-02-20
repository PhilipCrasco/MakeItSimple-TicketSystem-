﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class ApprovedTransferTicket
    {
       public class ApprovedTransferTicketCommand : IRequest<Result>
        {
            public Guid ? Transfer_By {  get; set; }
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public ICollection<ApproveTransferTicket> ApproveTransferTickets { get; set; }
            public class ApproveTransferTicket
            {
                public int RequestGeneratorId { get; set; }
            }

        }

        public class Handler : IRequestHandler<ApprovedTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                
                 foreach(var ticketRequest in command.ApproveTransferTickets)
                 {

                    var requestTicketExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == ticketRequest.RequestGeneratorId, cancellationToken);
                    if (requestTicketExist == null)
                    {
                        return Result.Failure(TransferTicketError.TicketIdNotExist());
                    }
                    var userTranferTicket = await _context.TransferTicketConcerns.Where(x => x.RequestGeneratorId == requestTicketExist.Id).ToListAsync();
                    var transferRequestTicketId = await _context.ApproverTicketings
                        .Where(x => x.RequestGeneratorId == requestTicketExist.Id && x.IsApprove == null).ToListAsync();

                    var selectTransferRequestId = transferRequestTicketId.FirstOrDefault(x => x.ApproverLevel == transferRequestTicketId.Min(x => x.ApproverLevel));

                    if(selectTransferRequestId != null)
                    {

                        selectTransferRequestId.IsApprove = true;

                        if(userTranferTicket.First().TicketApprover != command.Users)
                        {
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                        }

                        var userApprovalId = await _context.ApproverTicketings.Where(x => x.RequestGeneratorId == selectTransferRequestId.RequestGeneratorId).ToListAsync();

                        foreach(var concernTicket in userTranferTicket)
                        {
                            
                            var validateUserApprover = userApprovalId.FirstOrDefault(x => x.ApproverLevel == selectTransferRequestId.ApproverLevel + 1);

                            if(validateUserApprover != null)
                            {
                                concernTicket.TicketApprover = validateUserApprover.UserId;
                            }
                            else
                            {
                                concernTicket.TicketApprover = null;
                            }

                        }

                    }
                    else
                    {
                        foreach (var concernTicket in userTranferTicket)
                        {

                            if(TicketingConString.ApproverTransfer != command.Role)
                            {
                                return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                            }

                            concernTicket.IsTransfer = true;
                            concernTicket.TransferAt = DateTime.Now;
                            concernTicket.TransferBy = command.Transfer_By;
                            
                            var concernTicketById = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == concernTicket.TicketConcernId, cancellationToken);

                            concernTicketById.IsTransfer = true;
                            concernTicketById.TransferAt = DateTime.Now;
                            concernTicketById.TransferBy = command.Transfer_By;
                            concernTicketById.SubUnitId = concernTicket.SubUnitId;
                            concernTicketById.SubUnitId = concernTicket.SubUnitId;
                            concernTicketById.ChannelId = concernTicket.ChannelId;
                            concernTicketById.UserId = concernTicket.UserId;
                            concernTicketById.RequestGeneratorId = concernTicket.RequestGeneratorId;
                            concernTicketById.IsApprove = false;
                            concernTicketById.Remarks = TicketingConString.Transfer;
                        }


                    }

        
                   
                 }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}