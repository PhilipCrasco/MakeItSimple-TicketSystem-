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
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string AddedBy { get; set; }
            public DateTime Created_At { get; set; }
            public bool Is_Active { get; set; }
            public string ModifiedBy { get; set; }
            public DateTime Update_At { get; set;}
            public string User_Role_Name { get; set; }
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

                if(!string.IsNullOrEmpty(request.Search))
                {
                    userQuery = userQuery.Where(x => x.Fullname.Contains(request.Search) 
                    || x.Username.Contains(request.Search) || x.Email.Contains(request.Search) 
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
                    Fullname = x.Fullname,
                    Username = x.Username,
                    Email = x.Email,
                    AddedBy = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Is_Active = x.IsActive,
                    ModifiedBy = x.ModifiedByUser.Fullname,
                    Update_At = x.UpdatedAt,
                    User_Role_Name = x.UserRole.UserRoleName,
                    Permission =  x.UserRole.Permissions != null ? x.UserRole.Permissions : userPermissions

                });

                return await PagedList<GetUserResult>.CreateAsync(users, request.PageNumber , request.PageSize);

            }


        }



    }
}
