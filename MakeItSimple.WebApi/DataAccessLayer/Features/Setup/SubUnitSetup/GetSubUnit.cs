using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup
{
    public class GetSubUnit
    {

        public class GetSubUnitResult
        {
            public int Id { get; set; }

            public int SubUnit_No { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            
            public int ? UnitId { get; set; }
            public string Unit_Name {  get; set; }

            //public string Location_Name { get; set; }

            public bool Is_Active { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime SyncDate { get; set; }
            public string SyncStatus { get; set; }

            //public List<Location> Locations { get; set; }

            //public class Location
            //{
            //    public int LocationId { get; set; }
            //    public string Location_Code {  get; set; }
            //    public string Location_Name { get; set; }
            //}

            public List<User> users { get; set; }


            public class User
            {
                public Guid ? UserId { get; set; }
                public string Fullname { get; set; }
            }

            public List<Channel> channels { get; set; }

            public class Channel
            {
                public int ChannelId { get; set; }
                public string Channel_Name { get; set; }
            }

        }

        public class GetSubUnitQuery : UserParams, IRequest<PagedList<GetSubUnitResult>>
        {
            public string Search { get; set; }

            public bool ? Status { get; set; }

        }

        public class Handler : IRequestHandler<GetSubUnitQuery, PagedList<GetSubUnitResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetSubUnitResult>> Handle(GetSubUnitQuery request, CancellationToken cancellationToken)
            {
                IQueryable<SubUnit> subUnitQuery = _context.SubUnits.Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    subUnitQuery = subUnitQuery.Where(x => x.SubUnitCode.Contains(request.Search)
                    || x.SubUnitName.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    subUnitQuery = subUnitQuery.Where(x => x.IsActive == request.Status);
                }

                var result = subUnitQuery.Select(x => new GetSubUnitResult
                {
                    Id = x.Id,
                    SubUnit_No = x.SubUnitNo,
                    SubUnit_Code = x.SubUnitCode,
                    SubUnit_Name = x.SubUnitName,
                    UnitId = x.UnitId,
                    Unit_Name = x.Unit.UnitName,
                    Is_Active = x.IsActive,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,

                    SyncDate = x.SyncDate,
                    SyncStatus = x.SyncStatus,

                    users = x.Users.Where(x => x.IsActive == true).Select(x => new GetSubUnitResult.User
                    {
                        UserId = x.Id,
                        Fullname = x.Fullname,

                    }).ToList(),

                    channels = x.Channels.Where(x => x.IsActive == true).Select(x => new GetSubUnitResult.Channel
                    {
                        ChannelId = x.Id,

                        Channel_Name = x.ChannelName

                    }).ToList(),

                });

                return await PagedList<GetSubUnitResult>.CreateAsync(result, request.PageNumber, request.PageSize);

            }
        }
    }
}
