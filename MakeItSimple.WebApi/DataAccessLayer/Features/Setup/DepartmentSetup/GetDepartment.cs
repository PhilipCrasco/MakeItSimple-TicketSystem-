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
            public int ? BusinessUnitId { get; set; }
            public string BusinessUnit_Code {  get; set; }
            public string BusinessUnit_Name {  get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? SyncDate { get; set; }
            public string Sync_Status { get; set; }
            public int No_Of_Channels { get; set; }

            public bool Is_Active { get; set; }

            public List<Unit> Units { get; set; }

            public List<User> Users { get; set; }
            public List<Channel>Channels { get; set; }

            public class Unit
            {
                public int UnitId { get; set; }
                public string Unit_Name { get; set; }
            }

            public class User
            {
                public Guid? UserId { get; set; }
                public string EmpId { get; set; }
                public string FullName { get; set; }
            }

            public class Channel
            {
                public int ChannelId { get; set;}
                public string Channel_Name { get; set; } 

            }
    




        }

        public class GetDepartmentQuery : UserParams, IRequest<PagedList<GetDepartmentResult>>
        {
            public string Search { get; set; }
            public string Status { get; set; }

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
                IQueryable<Department> departmentsQuery = _context.Departments.Include(x => x.Units).Include(x => x.Channels)
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
                    BusinessUnitId = x.BusinessUnitId,
                    BusinessUnit_Code = x.BusinessUnit.BusinessCode,
                    BusinessUnit_Name = x.BusinessUnit.BusinessName,
                    Added_By = x.AddedByUser.Fullname,
                    No_Of_Channels = x.Channels.Count(),
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    Sync_Status = x.SyncStatus,
                    SyncDate = x.SyncDate,
                    Is_Active = x.IsActive,
                    Units = x.Units.Where(x => x.IsActive == true).Select(x => new GetDepartmentResult.Unit
                    {
                        UnitId = x.Id,
                        Unit_Name = x.UnitName,

                    }).ToList(),
                    Users = x.Users.Where(x => x.IsActive == true).Select(x => new GetDepartmentResult.User
                    {
                        UserId = x.Id,
                        EmpId = x.EmpId,
                        FullName = x.Fullname
                    
                    }).ToList(),
                    Channels = x.Channels.Where(x => x.IsActive == true).Select(x => new GetDepartmentResult.Channel
                    {
                        ChannelId = x.Id,
                        Channel_Name = x.ChannelName

                    }).ToList()
                    
                });
                

                return await PagedList<GetDepartmentResult>.CreateAsync(results, request.PageNumber , request.PageSize);
               
            }
        }
    }
}
