using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Experimental;
using System.Security.Cryptography.X509Certificates;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class RequestTicketNotification
    {

        public class RequestTicketNotificationResult
        {
            public int RequestTicketCount { get; set; }
        }

        public class RequestTicketNotificationQuery
        {
            public int ? RequestGeneratorId { get; set; }
            public int ChannelId { get; set; }
            public Guid ? UserId { get; set; }

            

            public class RequestTicketNotificationResultQuery : IRequest<Result>
            {

                public Guid? UserId { get; set; }
                public string Users { get; set; }
                public bool ? Status { get; set; }
                public bool ? IsReject { get; set; }
                public bool ? IsApprove { get; set; }

                

            }

            public class Handler : IRequestHandler<RequestTicketNotificationResultQuery, Result>
            {
                private readonly MisDbContext _context;

                public Handler(MisDbContext context)
                {
                    _context = context;
                }

                public async Task<Result> Handle(RequestTicketNotificationResultQuery request, CancellationToken cancellationToken)
                {

                    var query = await _context.TicketConcerns.GroupBy(x => x.RequestGeneratorId).ToListAsync();


                    if(request.Users  == TicketingConString.Users)
                    {
                        query = query.Where(x => x.First().UserId == request.UserId).ToList();
                    }

                    if(request.Status != null)
                    {
                        query = query.Where(x => x.First().IsActive == request.Status).ToList();
                    }

                    if(request.IsApprove != null)
                    {
                        query = query.Where(x => x.First().IsApprove  == request.IsApprove).ToList();
                    }

                    if (request.IsReject != null)
                    {
                        query = query.Where(x => x.First().IsReject == request.IsReject).ToList();
                    }

                    var notification = query.Select(x => new RequestTicketNotificationResult 
                    { 
                        RequestTicketCount = query.Count()

                    }).DistinctBy(x => x.RequestTicketCount).ToList();

                    return Result.Success(notification);
                }
            }


        }
    }
}
