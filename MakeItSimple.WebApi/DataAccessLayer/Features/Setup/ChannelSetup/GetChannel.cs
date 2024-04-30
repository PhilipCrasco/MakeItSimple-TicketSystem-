using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class GetChannel
    {

        public class GetChannelResult
        {
            public int Id { get; set; }
            public string Channel_Name { get; set; }

            public int No_Of_Members { get; set; }
            public bool  Is_Active { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

            public List<ChannelUser> channelUsers  { get; set; }

            public class ChannelUser
            {
                public int ChannelUserId {  get; set; }
                public Guid ? UserId { get; set; }
                public string Fullname { get; set; }
                public string UserRole { get; set; }
            }

        }

        public class GetChannelQuery : UserParams, IRequest<PagedList<GetChannelResult>>
        {
            public string Search { get; set; }
            public bool ? Status {  get; set; }
        }

        public class Handler : IRequestHandler<GetChannelQuery, PagedList<GetChannelResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetChannelResult>> Handle(GetChannelQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Channel> channelQuery = _context.Channels.Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser).Include(x => x.ChannelUsers);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    channelQuery = channelQuery.Where(x => x.ChannelName.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    channelQuery = channelQuery.Where(x => x.IsActive == request.Status);
                }

                var results = channelQuery.Select(x => new GetChannelResult
                {
                    Id = x.Id,
                    Channel_Name = x.ChannelName,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Updated_At = x.UpdatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    No_Of_Members = x.ChannelUsers.Count(),
                    Is_Active = x.IsActive,
                    channelUsers = x.ChannelUsers.Where(x => x.IsActive == true).Select(x => new GetChannelResult.ChannelUser
                    {
                        ChannelUserId = x.Id,
                        UserId = x.UserId,
                        Fullname = x.User.Fullname,
                        UserRole = x.User.UserRole.UserRoleName
                        

                    }).ToList()

                });

                return await PagedList<GetChannelResult>.CreateAsync(results , request.PageNumber , request.PageSize);  

            }
        }
    }
}
