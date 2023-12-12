using MakeItSimple.DataAccessLayer.Data;
using MakeItSimple.DataAccessLayer.Errors;
using MakeItSimple.Domain.Models.UserModel;
using MakeItSimple.Utility.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeItSimple.DataAccessLayer.Feature.UserFeatures
{
    public class GetUser
    {
       
        public class GetUserResult
        {
            public Guid id { get; set; }
            public string fullname { get; set; }
            public string username { get; set; }
            public string password { get; set; }

            public DateTime created_at { get; set; }

            public bool is_active { get; set; }


        }

        public class GetUsersQuery : IRequest<Result>
        {
            public bool ? status { get; set; }
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

                if (request.status != null)
                {
                    userQuery = userQuery.Where(x => x.IsActive == request.status);

                }

               
                var users = await userQuery.Select(x => new GetUserResult
                {

                    id = x.Id,
                    fullname = x.Fullname,
                    username = x.Username, 
                    password = x.Password,
                    created_at = x.CreatedAt


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
