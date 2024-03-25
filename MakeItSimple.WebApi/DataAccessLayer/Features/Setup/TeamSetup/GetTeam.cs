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
            public string Team_Name { get; set; }
            public int? SubUnitId { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }

            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }
            public bool Is_Active { get; set; }

        }

        public class GetTeamQuery : UserParams, IRequest<PagedList<GetTeamResult>>
        {
            public string Search { get; set; }
            public bool? Status { get; set; }
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

                IQueryable<Team> teamsQuery =  _context.Teams
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.SubUnit);


                if(!string.IsNullOrEmpty(request.Search))
                {
                    teamsQuery = teamsQuery.Where(x => x.TeamName.Contains(request.Search));

                }

                if(request.Status != null)
                {
                    teamsQuery = teamsQuery.Where(x => x.IsActive == request.Status);
                }


                var results = teamsQuery.Select(x => new GetTeamResult
                {

                    Id = x.Id,
                    Team_Name = x.TeamName,
                    SubUnitId = x.SubUnitId,
                    SubUnit_Code = x.SubUnit.SubUnitCode,
                    SubUnit_Name = x.SubUnit.SubUnitName,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt

                });

                return  await PagedList<GetTeamResult>.CreateAsync(results, request.PageNumber , request.PageSize); 

            }
        }
    }
}
