using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models;

namespace MakeItSimple.WebApi.DataAccessLayer.Feature.UserFeatures
{
    public class GetUser
    {
       
        public class GetUserResult
        {
            public Guid Id { get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public DateTime Created_At { get; set; }
            public bool Is_Active { get; set; }


        }

        public class GetUsersQuery : IRequest<Result>
        {
            public bool ? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetUsersQuery, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {

                IQueryable<User> userQuery = _context.Users;

                if (request.Status != null)
                {
                    userQuery = userQuery.Where(x => x.IsActive == request.Status);

                }

               
                var users = await userQuery.Select(x => new GetUserResult
                {

                    Id = x.Id,
                    Fullname = x.Fullname,
                    Username = x.Username, 
                    Password = x.Password,
                    Created_At = x.CreatedAt,
                    Is_Active = x.IsActive,


                }).ToListAsync();

                if(users is null )
                {
                    return Result.Failure(UserError.NoDataFound());
                }

                return Result.Success(users);   

            }


        }



    }
}
