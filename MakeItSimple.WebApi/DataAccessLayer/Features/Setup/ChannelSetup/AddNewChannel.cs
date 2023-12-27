using MakeItSimple.WebApi.Models.Setup.TeamSetup;
using MakeItSimple.WebApi.Models;
using MediatR;
using MakeItSimple.WebApi.Common;
using System.Security.Policy;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class AddNewChannel
    {
        public class AddNewChannelResult
        {
            public int Id {  get; set; }
            public string Channel_Name { get; set; }
            public string Team { get; set; }
           
            public string User { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }

        }


        public class AddNewChannelCommand : IRequest<Result>
        {
            public string Channel_Name { get; set; }
            public int TeamId { get; set; }

            public Guid ? UserId { get; set; }
            public Guid ? Added_By { get; set; }
           
        }


        public class Handler : IRequestHandler<AddNewChannelCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewChannelCommand command, CancellationToken cancellationToken)
            {

                var channels = new Channel
                {
                    ChannelName = command.Channel_Name,
                    TeamId = command.TeamId, 
                    UserId = command.UserId,
                    AddedBy = command.Added_By,

                };

                await _context.Channels.AddAsync(channels , cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var addedby = await _context.Users.FirstOrDefaultAsync(x => x.Id == channels.AddedBy, cancellationToken);
                var teamName = await _context.Teams.FirstOrDefaultAsync(x => x.Id == channels.TeamId , cancellationToken);
                var fullname = await _context.Users.FirstOrDefaultAsync(x => x.Id == channels.UserId, cancellationToken);

                var results = new AddNewChannelResult
                {
                    Id = channels.Id,
                    Channel_Name = channels.ChannelName,    
                    Team = channels.Team.TeamName,
                    User = channels.User.Fullname,
                    Added_By = addedby.Fullname,
                   Created_At = channels.CreatedAt


                };

                return Result.Success(results);

            }
        }
    }
}
