using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.CancelReTicket.CancelReTicketCommand.CancelReTicketConcern;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class CancelReTicket
    {
        public class CancelReTicketCommand : IRequest<Result>
        {
            public List<CancelReTicketConcern> CancelReTicketConcerns { get; set; }

            public class CancelReTicketConcern
            {
                public int TicketGeneratorId { get; set; }

                public List<CancelReTicketById> CancelReTicketByIds { get; set; }
                public class CancelReTicketById
                {
                    public int? ReTicketConcernId { get; set; }
                }

            }

            public class Handler : IRequestHandler<CancelReTicketCommand, Result>
            {
                private readonly MisDbContext _context;

                public Handler(MisDbContext context)
                {
                    _context = context;
                }

                public async Task<Result> Handle(CancelReTicketCommand command, CancellationToken cancellationToken)
                {

                    foreach (var transferTicket in command.CancelReTicketConcerns)
                    {
                        var reTicketQuery = await _context.ReTicketConcerns.Where(x => x.TicketGeneratorId == transferTicket.TicketGeneratorId).ToListAsync();

                        if (reTicketQuery == null)
                        {
                            return Result.Failure(ReTicketConcernError.TicketIdNotExist());
                        }


                        if (transferTicket.CancelReTicketByIds.Count(x => x.ReTicketConcernId != null) <= 0)
                        {
                            foreach (var reTicketList in reTicketQuery)
                            {
                                var reTicketConcernRequest = await _context.TicketConcerns.Where(x => x.Id == reTicketList.TicketConcernId).ToListAsync();

                                foreach (var reTicketConcern in reTicketConcernRequest)
                                {
                                    reTicketConcern.IsReTicket = null;
                                }

                                _context.Remove(reTicketList);
                            }
                        }

                        foreach (var reTicketId in transferTicket.CancelReTicketByIds)
                        {
                            var reTicketConcernId = reTicketQuery.FirstOrDefault(x => x.Id == reTicketId.ReTicketConcernId);
                            if (reTicketConcernId != null)
                            {
                                var reTicketConcernRequest = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == reTicketConcernId.TicketConcernId, cancellationToken);

                                reTicketConcernRequest.IsReTicket = null;

                                _context.Remove(reTicketConcernId);
                            }
                            else
                            {
                                return Result.Failure(ReTicketConcernError.ReTicketIdNotExist());
                            }

                        }



                    }


                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success();
                }
            }
        }
    }
}
