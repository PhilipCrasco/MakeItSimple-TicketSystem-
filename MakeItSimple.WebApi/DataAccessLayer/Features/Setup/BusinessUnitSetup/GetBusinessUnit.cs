using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.BusinessUnitSetup
{
    public class GetBusinessUnit
    {

        public class GetBussinessUnitResults
        {
            public int Id { get; set; } 
            public int? Business_No { get; set; }
            public string Business_Code { get; set; }
            public string Business_Name { get; set; }
            public  int ? CompanyId { get; set; }
            public string Company_Name { get; set; }
            public string AddedBy { get; set; }
            public DateTime CreatedAt { get; set; } 
            public DateTime? UpdatedAt { get; set; }
            public virtual string ModifiedBy { get; set; }
            public DateTime SyncDate { get; set; }
            public string SyncStatus { get; set; }
            public bool IsActive { get; set; }
            public ICollection<Department> Departments { get; set; }
            public class Department
            {
                public int ? DepartmentId { get; set; }
                public string Department_Code { get; set; }

                public string Department_Name { get; set; }

            }

        }

         public class GetBussinessUnitQuery : UserParams, IRequest<PagedList<GetBussinessUnitResults>>
         {
            public string Search { get; set; }
            public bool ? Status { get; set; }
         }

        public class Handler : IRequestHandler<GetBussinessUnitQuery, PagedList<GetBussinessUnitResults>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetBussinessUnitResults>> Handle(GetBussinessUnitQuery request, CancellationToken cancellationToken)
            {
                IQueryable<BusinessUnit> businessUnitQuery = _context.BusinessUnits
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    businessUnitQuery = businessUnitQuery.Where(x => x.BusinessCode.Contains(request.Search)
                    || x.BusinessName.Contains(request.Search));
                }

                if(request.Status != null)
                {
                    businessUnitQuery = businessUnitQuery.Where(x => x.IsActive == request.Status);
                }

                var results = businessUnitQuery.Select(x => new GetBussinessUnitResults
                {
                    
                    Id = x.Id,
                    Business_No = x.Business_No,    
                    Business_Code = x.BusinessCode,
                    Business_Name = x.BusinessName,
                    CompanyId = x.CompanyId,
                    Company_Name = x.Company.CompanyName,
                    AddedBy = x.AddedByUser.Fullname,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedByUser.Fullname,
                    UpdatedAt = x.UpdatedAt,
                    SyncDate = x.SyncDate,
                    SyncStatus = x.SyncStatus,
                    IsActive = x.IsActive,
                    Departments = x.Departments.Select(x => new GetBussinessUnitResults.Department
                    {
                        DepartmentId = x.Id,
                        Department_Code = x.DepartmentCode,
                        Department_Name = x.DepartmentName

                    }).ToList()
                    


                });


                return await PagedList<GetBussinessUnitResults>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
