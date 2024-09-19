using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup
{
    public class GetUserDepartment
    {
        public record GetUserDepartmentResult
        {
            public int departmentId { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }

            public string Role { get; set; }

        }


        public class GetUserDepartmentCommand : IRequest<Result>
        {
            public int DepartmentId { get; set; }
        }

        public class Handler : IRequestHandler<GetUserDepartmentCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetUserDepartmentCommand request, CancellationToken cancellationToken)
            {

                var result = await _context.Users
                    .Include(x => x.UserRole)
                    .Where(x => x.IsActive == true && x.DepartmentId == request.DepartmentId)
                    .Select(x => new GetUserDepartmentResult
                    {
                        departmentId = x.DepartmentId.Value,
                        UserId = x.Id,
                        Fullname = x.Fullname,
                        Role = x.UserRole.UserRoleName,

                    }).ToListAsync();
                
                return  Result.Success(result);
            }
        }
    }
}
