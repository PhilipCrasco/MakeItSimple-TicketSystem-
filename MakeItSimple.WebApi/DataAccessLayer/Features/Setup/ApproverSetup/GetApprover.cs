using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup
{
    public class GetApprover
    {
        public class GetApproverResult
        {
            public int ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public bool Is_Active { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By {  get; set; }
            public DateTime ? Updated_At { get; set; }
            public List<Approver> Approvers {  get; set; }

            public class Approver
            {
                public Guid ? UserId { get; set; }

                public string Fullname { get; set; }
                public int ? ApproverLevel { get; set; }
            }
        }

        public class GetApproverQuery : UserParams, IRequest<PagedList<GetApproverResult>>
        {
            public string Search {  get; set; }
            public bool? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetApproverQuery, PagedList<GetApproverResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetApproverResult>> Handle(GetApproverQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Approver> approverQuery = _context.Approvers.Include(x => x.User).Include(x => x.Channel)
                                                      .Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    approverQuery = approverQuery.Where(x => x.Channel.ChannelName.Contains(request.Search));
                }

                if(request.Status != null)
                {
                    approverQuery = approverQuery.Where(x => x.IsActive == request.Status);
                }

                var result = approverQuery.GroupBy(x => x.ChannelId)
                                .Select(x => new GetApproverResult
                                {
                                    ChannelId = x.Key,
                                    Channel_Name = x.First().Channel.ChannelName,
                                    Is_Active = x.First().IsActive,
                                    Added_By = x.First().AddedByUser.Fullname,
                                    Created_At = x.First().CreatedAt,
                                    Modified_By = x.First().ModifiedByUser.Fullname,
                                    Updated_At = x.First().UpdatedAt,
                                    Approvers = x.Select(x => new GetApproverResult.Approver
                                    {
                                       UserId = x.UserId,
                                       Fullname = x.User.Fullname,
                                       ApproverLevel = x.ApproverLevel

                                    }).ToList(),
                                    
                                });

                return await PagedList<GetApproverResult>.CreateAsync(result , request.PageNumber, request.PageSize);
                                         
            }
        }
    }
}
