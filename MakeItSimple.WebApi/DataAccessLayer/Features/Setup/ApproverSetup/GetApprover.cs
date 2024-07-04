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
            public int ? SubUnitId { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public bool Is_Active { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By {  get; set; }
            public DateTime ? Updated_At { get; set; }
            public List<Approver> Approvers {  get; set; }

            public class Approver
            {
                public int ? ApproverId {  get; set; }
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
                IQueryable<Approver> approverQuery = _context.Approvers.Include(x => x.User).Include(x => x.SubUnit)
                                                      .Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    approverQuery = approverQuery.Where(x => x.Channel.ChannelName.Contains(request.Search));
                }


                if(request.Status != null)
                {
                    approverQuery = approverQuery.Where(x => x.IsActive == request.Status);
                }

                var result = approverQuery.GroupBy(x => x.SubUnitId)
                                .Select(x => new GetApproverResult
                                {
                                    SubUnitId = x.Key,
                                   
                                    SubUnit_Code = x.First().SubUnit.SubUnitCode,
                                    SubUnit_Name = x.First().SubUnit.SubUnitName,
                                    Is_Active = x.First().IsActive,
                                    Added_By = x.First().AddedByUser.Fullname,
                                    Created_At = x.First().CreatedAt,
                                    Modified_By = x.First().ModifiedByUser.Fullname,
                                    Updated_At = x.First().UpdatedAt,
                                    Approvers = x.Select(x => new GetApproverResult.Approver
                                    {
                                       ApproverId = x.Id,
                                       UserId = x.UserId,
                                       Fullname = x.User.Fullname,
                                       ApproverLevel = x.ApproverLevel

                                    }).OrderBy(x => x.ApproverLevel).ToList(),
                                    
                                });

                return await PagedList<GetApproverResult>.CreateAsync(result , request.PageNumber, request.PageSize);
                                         
            }
        }
    }
}
