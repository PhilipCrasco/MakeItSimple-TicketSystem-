using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class CancelTransferTicket
    {
        public class CancelTransferTicketCommand : IRequest<Result>
        {
           public List<CancelTransferTicketConcern> CancelTransferTicketConcerns { get; set; }

            public class CancelTransferTicketConcern
            {
                public int RequestGeneratorId { get; set; }

                public List<CancelTransferTicketById>  CancelTransferTicketByIds { get; set; }
                public class CancelTransferTicketById
                {
                    public int? TransferTicketConcernId { get; set; }
                }
            }
        }

        public class Handler : IRequestHandler<CancelTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelTransferTicketCommand command, CancellationToken cancellationToken)
            {

                foreach(var transferTicket in command.CancelTransferTicketConcerns)
                {
                        var transferTicketQuery = await _context.TransferTicketConcerns.Where(x => x.RequestGeneratorId == transferTicket.RequestGeneratorId ).ToListAsync();

                        if (transferTicketQuery == null)
                        {
                            return Result.Failure(TransferTicketError.TicketIdNotExist());
                        }


                    if(transferTicket.CancelTransferTicketByIds.Count <= 0)
                    {
                        foreach (var transferList in transferTicketQuery)
                        {
                            var transferConcernRequest = await _context.TicketConcerns.Where(x => x.Id == transferList.TicketConcernId).ToListAsync();

                            foreach (var transferConcern in transferConcernRequest)
                            {
                                transferConcern.IsTransfer = null;
                            }

                            _context.Remove(transferList);
                        }
                    }
                      
                    foreach (var transferId in transferTicket.CancelTransferTicketByIds)
                    {
                        var transferconcernId = transferTicketQuery.FirstOrDefault(x => x.Id == transferId.TransferTicketConcernId);
                        if (transferconcernId != null)
                        {
                            var transferConcernRequest = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == transferconcernId.TicketConcernId, cancellationToken);

                            transferConcernRequest.IsTransfer = null;
                            

                            _context.Remove(transferconcernId);
                        }
                        else
                        {
                            return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());
                        }

                    }

                    

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
