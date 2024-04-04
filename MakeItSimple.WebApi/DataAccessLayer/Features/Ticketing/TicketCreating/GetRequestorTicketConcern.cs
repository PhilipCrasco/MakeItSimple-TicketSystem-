
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class GetRequestorTicketConcern
    {
        public class GetRequestorTicketConcernResult
        {
            public int ? RequestGeneratorId { get; set; }
            public Guid ? UserId { get; set; }  

            public string EmpId { get; set; }

            public string FullName { get; set; }
            public string Concern { get; set; }

                public int? RequestConcernId { get; set; }

                public string Concern_Status { get; set; }
                public bool ? Is_Done { get; set; }
                public string Remarks { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime ? updated_At { get; set; }

            

        }

        public class GetRequestorTicketConcernQuery :UserParams , IRequest<PagedList<GetRequestorTicketConcernResult>>
        {
            public string Requestor { get; set; }

            public Guid? UserId { get; set; }
            public string Concern_Status { get; set ; } 
            public string Search { get; set; }
            public bool ? Status { get; set; }


        }

        public class Handler : IRequestHandler<GetRequestorTicketConcernQuery, PagedList<GetRequestorTicketConcernResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetRequestorTicketConcernResult>> Handle(GetRequestorTicketConcernQuery request, CancellationToken cancellationToken)
            {
                IQueryable<RequestConcern> requestConcernsQuery = _context.RequestConcerns.Include(x => x.User)
                     .Include(x => x.AddedByUser)
                     .Include(x => x.ModifiedByUser);


                if (request.Status != null)
                {
                    requestConcernsQuery = requestConcernsQuery.Where(x => x.IsActive == request.Status);
                }

                if (!request.Search.IsNullOrEmpty())
                {
                    requestConcernsQuery = requestConcernsQuery.Where(x => x.User.Fullname.Contains(request. Search)
                    && x.Id.ToString().Contains(request.Search));
                }

                if (request.Requestor == TicketingConString.Requestor)
                {
                    requestConcernsQuery = requestConcernsQuery.Where(x => x.UserId == request.UserId);
                }


                if(request.Concern_Status != null)
                {
                    if(request.Concern_Status == TicketingConString.Approval)
                    {
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket);
                    }
                    else if(request.Concern_Status == TicketingConString.OnGoing)
                    {
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing);
                    }
                    else if(request.Concern_Status == TicketingConString.Done)
                    {
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.Done);
                    }
                    else
                    {
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestGeneratorId == null);
                    }

                }


                
                var results = requestConcernsQuery.Select(x => new GetRequestorTicketConcernResult
                {
                    RequestGeneratorId = x.RequestGeneratorId,
                    UserId = x.UserId,
                    EmpId = x.User.EmpId,
                    FullName = x.User.Fullname,
                    Concern = x.Concern,
                    RequestConcernId = x.Id,
                    Concern_Status = x.ConcernStatus,
                    Is_Done = x.IsDone,
                    Remarks = x.Remarks,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    updated_At = x.UpdatedAt

                });

                return await PagedList<GetRequestorTicketConcernResult>.CreateAsync(results , request.PageNumber , request.PageSize);
            }
        }
    }
}
