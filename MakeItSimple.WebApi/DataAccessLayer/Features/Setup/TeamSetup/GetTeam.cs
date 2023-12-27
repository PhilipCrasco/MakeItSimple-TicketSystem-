using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.TeamSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup
{
    public class GetTeam
    {

        public class GetTeamResult
        {
            public int Id { get; set; }
            public string Team_Code { get; set; }
            public string Team_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

            public List<Channel> channels{ get; set; }

            public class Channel
            {
                public int Id { get; set; }
                public string Channel_Name { get; set; }
            } 

        }

        public class GetTeamQuery : UserParams, IRequest<PagedList<GetTeamResult>>
        {
            public string Search { get; set; }

            public bool ? Status { get; set; }

        }

        public class Handler : IRequestHandler<GetTeamQuery, PagedList<GetTeamResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetTeamResult>> Handle(GetTeamQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Team> teamQuery = _context.Teams.Include(x => x.Channels).Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    teamQuery = teamQuery.Where(x => x.TeamCode.Contains(request.Search) 
                    || x.TeamName.Contains(request.Search));
                }

                if(request.Status != null)
                {
                     teamQuery = teamQuery.Where(x => x.IsActive == request.Status);
                }

                var result =  teamQuery.Select(x => new GetTeamResult
                {
                    Id = x.Id,
                    Team_Code = x.TeamCode,
                    Team_Name = x.TeamName,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    channels = x.Channels.Select(x => new GetTeamResult.Channel
                    {
                        Id = x.Id,
                        Channel_Name = x.ChannelName

                    }).ToList()


                });

                return await PagedList<GetTeamResult>.CreateAsync(result , request.PageNumber , request.PageSize);

            }
        }
    }
}
