using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class GetMember
    {
        public class GetMemberResult
        {
            public Guid? UserId { get; set; }
            public string EmpId { get; set; }
            public  string FullName { get; set; }
            public string UserRole {  get; set; }

        }

        public class GetMemberQuery : IRequest<Result>
        {
            public int ? ChannelId { get; set; }
        }

        public class Handler : IRequestHandler<GetMemberQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetMemberQuery request, CancellationToken cancellationToken)
            {
                var channelUsers = await _context.ChannelUsers
                    .Where(x => x.ChannelId == request.ChannelId)
                    .ToListAsync(); 

                var selectedChannelUsers = channelUsers.Select(x => x.UserId);

                var results = await _context.Users
                    .Where(x => !selectedChannelUsers.Contains(x.Id))
                    .Select(x => new GetMemberResult
                    {

                        UserId = x.Id,
                        EmpId = x.EmpId,
                        FullName = x.Fullname,
                        UserRole = x.UserRole.UserRoleName

                    }).ToListAsync();   

                return Result.Success(results);
            }
        }
    }
}
