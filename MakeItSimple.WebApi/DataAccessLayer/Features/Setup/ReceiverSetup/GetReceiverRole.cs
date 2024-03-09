using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup
{
    public class GetReceiverRole
    {
        public class GetReceiverRoleResult
        {
            public Guid? UserId { get; set; }

            public string EmpId { get; set; }

            public string FullName { get; set; }

        }

        public class GetReceiverRoleQuery : IRequest<Result> { }

        public class Handler : IRequestHandler<GetReceiverRoleQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetReceiverRoleQuery request, CancellationToken cancellationToken)
            {
                var users = await _context.Users.Include(x => x.UserRole).Where(x => x.UserRole.UserRoleName == TicketingConString.Receiver)
                    .Select(x => new GetReceiverRoleResult
                    {
                        UserId = x.Id,
                        EmpId = x.EmpId,
                        FullName = x.Fullname

                    }).ToListAsync();


                return Result.Success(users);

            }
        }
    }
}
