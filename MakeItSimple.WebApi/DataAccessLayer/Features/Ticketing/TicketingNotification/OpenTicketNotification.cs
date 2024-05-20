using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.RequestTicketNotification;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class OpenTicketNotification
    {
        public class OpenTicketNotificationResult
        {
            public int OpenTicketCount { get; set; }
            
        }

        public class OpenTicketNotificationQuery
        {
             public int ? TicketGeneratorId { get; set; }
        }

        public class OpenTicketNotificationResultQuery : IRequest<Result>
        {
            public bool? Status { get; set; }
            public string Receiver { get; set; }
            public string Users { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
        }


        public class Handler : IRequestHandler<OpenTicketNotificationResultQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(OpenTicketNotificationResultQuery request, CancellationToken cancellationToken)
            {
                var query = await _context.TicketConcerns
                    .Include(x => x.RequestorByUser)
                    .Where(x => x.IsApprove == true && x.IsClosedApprove != true)
                    .GroupBy(x => x.RequestTransactionId)
                    .ToListAsync();

                var getQuery = query.Select(x => x.First().RequestorByUser.BusinessUnitId);

                var businessUnitList = await _context.BusinessUnits
                    .FirstOrDefaultAsync(x => getQuery.Contains(x.Id));

                var receiverList = await _context.Receivers
                    .FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);

                var fillterApproval = query.Select(x => x.First().RequestTransactionId);

                if (request.Status != null)
                {
                    query = query.Where(x => x.First().IsActive == true).ToList();
                }


                if (request.Users == TicketingConString.Users)
                {
                    query = query.Where(x => x.First().UserId == request.UserId).ToList();
                }

                if (request.Receiver == TicketingConString.Receiver)
                {
                    if (request.Role == TicketingConString.Receiver && request.UserId == receiverList.UserId)
                    {

                        var receiver = await _context.TicketConcerns
                            .Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId)
                            .ToListAsync();

                        var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);

                        query = query.Where(x => receiverContains.Contains(x.First().RequestorByUser.BusinessUnitId))
                            .ToList();

                    }
                    else
                    {
                        query = query.Where(x => x.First().RequestTransactionId == null).ToList();
                    }

                }

                var notification = query.Select(x => new OpenTicketNotificationResult
                {
                    OpenTicketCount = query.Count()

                }).DistinctBy(x => x.OpenTicketCount).ToList();

                return Result.Success(notification);
            }
        }
    }
}
