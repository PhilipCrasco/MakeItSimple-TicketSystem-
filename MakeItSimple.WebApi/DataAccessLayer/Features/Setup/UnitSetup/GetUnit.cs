using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.UnitSetup
{
    public class GetUnit
    {
        public class GetUnitResult
        {
            public int Id { get; set; }

            public int Unit_No { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }

            public string Department_Name { get; set; }

            public bool Is_Active { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

            public DateTime SyncDate { get; set; }
            public string SyncStatus { get; set; }

            public List<User> users { get; set; }

            public List<SubUnitList> SubUnitLists { get; set; }

            public class User
            {
                public Guid? UserId { get; set; }
                public string Fullname { get; set; }
            }

            public class SubUnitList
            {
                public int Id { get; set; }
                public string SubUnit_Name { get; set; }

            }


        }


        public class GetUnitQuery : UserParams , IRequest<PagedList<GetUnitResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetUnitQuery, PagedList<GetUnitResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetUnitResult>> Handle(GetUnitQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Models.Setup.UnitSetup.Unit> unitQuery = _context.Units.Include(x => x.SubUnits).Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    unitQuery = unitQuery.Where(x => x.UnitCode.Contains(request.Search)
                    || x.UnitName.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    unitQuery = unitQuery.Where(x => x.IsActive == request.Status);
                }

                var result = unitQuery.Select(x => new GetUnitResult
                {
                    Id = x.Id,
                    Unit_No = x.UnitNo,
                    Unit_Code = x.UnitCode,
                    Unit_Name = x.UnitName,
                    Department_Name = x.Department.DepartmentName,
                    Is_Active = x.IsActive,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    SyncDate = x.SyncDate,
                    SyncStatus = x.SyncStatus,
                    users = x.Users.Where(x => x.IsActive == true).Select(x => new GetUnitResult.User
                    {
                        UserId = x.Id,
                        Fullname = x.Fullname,

                    }).ToList(),

                    SubUnitLists = x.SubUnits.Where(x => x.IsActive == true).Select(x => new GetUnitResult.SubUnitList
                    {
                        Id = x.Id,
                        SubUnit_Name = x.SubUnitName

                    }).ToList(),

                });

                return await PagedList<GetUnitResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
