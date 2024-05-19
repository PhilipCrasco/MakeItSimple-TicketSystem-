using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.ProjectSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ProjectSetup
{
    public class GetProject
    {
        public class GetProjectResult
        {
            public int ProjectId { get; set; }
            public string Project_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }

            public List<Channel> Channels { get; set; }

            public class Channel
            {
                public int ChannelId { get; set; }
                public string Channel_Name { get; set; }
            }

        }

        public class GetProjectQuery : UserParams, IRequest<PagedList<GetProjectResult>>
        {
            public string Search {  get; set; }
            public bool ? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetProjectQuery, PagedList<GetProjectResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetProjectResult>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Project> projectQuery = _context.Projects
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.Channels);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    projectQuery = projectQuery.Where(x => x.ProjectName.Contains(request.Search));
                }

                if(request.Status != null)
                {
                    projectQuery = projectQuery.Where(x => x.IsActive == request.Status);
                }


                var result = projectQuery.Select(x => new GetProjectResult
                {
                   ProjectId = x.Id,
                   Project_Name = x.ProjectName,
                   Added_By = x.AddedByUser.Fullname,
                   Created_At = x.CreatedAt,
                   Modified_By = x.ModifiedByUser.Fullname,
                   Updated_At = x.UpdatedAt,
                   Channels = x.Channels.Select(x => new GetProjectResult.Channel
                   {
                       ChannelId = x.Id,
                       Channel_Name = x.ChannelName,

                   }).ToList()
                });

                return await PagedList<GetProjectResult>.CreateAsync(result ,request.PageNumber , request.PageSize);
            }
        }
    }
}
