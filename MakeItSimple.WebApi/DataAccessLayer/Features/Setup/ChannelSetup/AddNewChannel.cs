using MakeItSimple.WebApi.Models.Setup.TeamSetup;
using MakeItSimple.WebApi.Models;
using MediatR;
using MakeItSimple.WebApi.Common;
using System.Security.Policy;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class AddNewChannel
    {
        public class AddNewChannelResult
        {
            public int Id {  get; set; }
            public string Channel_Name { get; set; }
            public int SubUnitId { get; set; }
            public Guid ? Added_By { get; set; }
            public DateTime Created_At { get; set; }

        }


        public class AddNewChannelCommand : IRequest<Result>
        {
            public string Channel_Name { get; set; }
            public int SubUnitId { get; set; }
            //public Guid ? UserId { get; set; }
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

                var ChannelNameAlreadyExist = await _context.Channels.FirstOrDefaultAsync(x => x.ChannelName == command.Channel_Name, cancellationToken);

                if (ChannelNameAlreadyExist != null)
                {
                    return Result.Failure(ChannelError.ChannelNameAlreadyExist(command.Channel_Name));
                }

                var SubUnitNotExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);

                if (SubUnitNotExist == null)
                {
                    return Result.Failure(ChannelError.SubUnitNotExist());
                }

                var channels = new Channel
                {
                    ChannelName = command.Channel_Name,
                    SubUnitId = command.SubUnitId, 
                    AddedBy = command.Added_By,

                };

                await _context.Channels.AddAsync(channels , cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var results = new AddNewChannelResult
                {
                    Id = channels.Id,
                    Channel_Name = channels.ChannelName,
                    SubUnitId = channels.SubUnitId,
                    Added_By = channels.AddedBy,
                   Created_At = channels.CreatedAt
                };

                return Result.Success(results);

            }
        }
    }
}
