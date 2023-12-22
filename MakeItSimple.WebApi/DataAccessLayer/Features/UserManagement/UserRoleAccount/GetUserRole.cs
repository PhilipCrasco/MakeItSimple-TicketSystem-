using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount
{
    public class GetUserRole
    {
        public class GetUserRoleResult
        {
            public int Id { get; set; }
            public string User_Role_Name { get; set; }
            public ICollection<string> Permissions { get; set; }

            public Guid ? Added_by { get; set; }

            public DateTime Created_At { get; set; }

            public Guid ? Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }

            public bool Is_Tagged { get; set; }

            public List<Users> users {  get; set; } 

            public class Users
            {
                public Guid UserId { get; set; }
                public string Fullname { get; set; }
            }

        }

        public class GetUserRoleQuery : UserParams , IRequest<PagedList<GetUserRoleResult>>
        {
            public string Search { get; set; }

            public bool ? Status { get; set; }

            public bool ? Is_Tagged { get; set; }
        }

        public class Handler : IRequestHandler<GetUserRoleQuery, PagedList<GetUserRoleResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                 _context = context;
            }

            public async Task<PagedList<GetUserRoleResult>> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
            {
                IQueryable<UserRole> userRole = _context.UserRoles.Include(x => x.Users).Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    userRole = userRole.Where(x => x.UserRoleName.Contains(request.Search));
                }

                if(request.Status != null)
                {
                    userRole = userRole.Where(x => x.IsActive == request.Status);
                }

                if(request.Is_Tagged != null)
                {

                    userRole = request.Is_Tagged == true ? userRole.Where(x => x.Users.FirstOrDefault().Fullname != null)
                        : userRole.Where(x => x.Users.FirstOrDefault().Fullname == null);

                }


                var result = userRole.Select(x => new GetUserRoleResult
                {
                    Id = x.Id,
                    User_Role_Name = x.UserRoleName,
                    Permissions = x.Permissions,
                    Added_by = x.AddedBy,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedBy,
                    Updated_At = x.UpdatedAt, 
                    Is_Tagged = x.Users.FirstOrDefault().Fullname != null ? true : false,
                    users = x.Users.Select(x => new GetUserRoleResult.Users
                    {

                        UserId = x.Id,
                        Fullname = x.Fullname

                    }).ToList(),

                });


                return await PagedList<GetUserRoleResult>.CreateAsync(result, request.PageNumber , request.PageSize);



            }
        }


    }
}
