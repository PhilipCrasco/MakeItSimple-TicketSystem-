using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Common.Pagination;

namespace MakeItSimple.WebApi.DataAccessLayer.Feature.UserFeatures
{
    public class GetUser
    {
       
        public class GetUserResult
        {
            public Guid Id { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public bool Is_Active { get; set; }
            public string Modified_By { get; set; }
            public DateTime ? Update_At { get; set;}

            public int? UserRoleId { get; set; }
            public string User_Role_Name { get; set; }

            public int? DepartmentId { get; set; }
            public string Department_Name { get; set; }

            public int? CompanyId { get; set; }
            public string Company_Name { get; set; }

            public int ? LocationId { get; set; }
            public string Location_Name { get; set; }

            public int ? BusinessUnitId {  get; set; }
            public string Business_Name { get; set; }
            
            public int ? UnitId { get; set; }
            public string Unit_Name {  get; set; }
            

            public int ? SubUnitId { get; set; }
            public string SubUnit_Name { get; set; }
            public ICollection<string> Permission {  get; set; }


        }

        public class GetUsersQuery : UserParams, IRequest<PagedList<GetUserResult>>
        {
            public bool ? Status { get; set; }
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<GetUsersQuery, PagedList<GetUserResult>>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetUserResult>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {

                IQueryable<User> userQuery = _context.Users
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.UserRole);

                    //.Include(x => x.acc);
                    

                if(!string.IsNullOrEmpty(request.Search))
                {
                    userQuery = userQuery.Where(x => x.Fullname.Contains(request.Search) 
                    || x.UserRole.UserRoleName.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    userQuery = userQuery.Where(x => x.IsActive == request.Status);

                }


                var userPermissions = new List<string>();

                var users =  userQuery.Select(x => new GetUserResult
                {

                    Id = x.Id,
                    EmpId = x.EmpId,
                    Fullname = x.Fullname,
                    Username = x.Username,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Is_Active = x.IsActive,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Update_At = x.UpdatedAt,
                    UserRoleId = x.UserRoleId,
                    User_Role_Name = x.UserRole.UserRoleName,
                    DepartmentId = x.DepartmentId,
                    Department_Name = x.Department.DepartmentName,
                    SubUnitId = x.SubUnitId,
                    SubUnit_Name = x.SubUnit.SubUnitName,
                    CompanyId = x.CompanyId,
                    Company_Name = x.Company.CompanyName,
                    LocationId = x.LocationId,
                    Location_Name = x.Location.LocationName,
                    BusinessUnitId = x.BusinessUnitId,
                    Business_Name = x.BusinessUnit.BusinessName,
                    UnitId = x.UnitId,
                    Unit_Name = x.Units.UnitName,
                    Permission =  x.UserRole.Permissions != null ? x.UserRole.Permissions : userPermissions

                });

                return await PagedList<GetUserResult>.CreateAsync(users, request.PageNumber , request.PageSize);

            }


        }



    }
}
