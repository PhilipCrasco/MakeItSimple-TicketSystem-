using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup
{
    public class GetDepartment
    {
        public class GetDepartmentResult
        {
            public int Id { get; set; }
            public int Department_No { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? SyncDate { get; set; }
            public string Sync_Status { get; set; }

            public List<SubUnit> subUnits { get; set; }

            public class SubUnit
            {
                public int SubUnitId { get; set; }
                public string SubUnit_Name { get; set; }
            }

    




        }

        public class GetDepartmentQuery : UserParams, IRequest<PagedList<GetDepartmentResult>>
        {
            public string Search { get; set; }

        }

        public class Handler : IRequestHandler<GetDepartmentQuery, PagedList<GetDepartmentResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetDepartmentResult>> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Department> departmentsQuery = _context.Departments.Include(x => x.SubUnits)
                    .Include(x => x.Users).Include(x => x.ModifiedByUser).Include(x => x.AddedByUser);
                

                if(!string.IsNullOrEmpty(request.Search))
                {
                    departmentsQuery = departmentsQuery.Where(x => x.DepartmentCode.Contains(request.Search)
                    || x.DepartmentName.Contains(request.Search));
                }

                var results = departmentsQuery.Select(x => new GetDepartmentResult
                {
                    Id = x.Id,
                    Department_No = x.DepartmentNo,
                    Department_Code = x.DepartmentCode,
                    Department_Name = x.DepartmentName,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    Sync_Status = x.SyncStatus,
                    SyncDate = x.SyncDate,
                    subUnits = x.SubUnits.Select(x => new GetDepartmentResult.SubUnit
                    {
                        SubUnitId = x.Id,
                        SubUnit_Name = x.SubUnitName,

                    }).ToList()
                    
                });
                

                return await PagedList<GetDepartmentResult>.CreateAsync(results, request.PageNumber , request.PageSize);
               
            }
        }
    }
}
