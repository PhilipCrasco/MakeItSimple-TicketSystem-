using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class UpdateChannel
    {

        public class UpdateChannelResult
        {
            public int Id { get; set; }
            public string Channel_Name { get; set; }
            public int ? SubUnitId { get; set; }
            public Guid? Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }
        }

        public class UpdateChannelCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string Channel_Name { get; set; }
            public int SubUnitId { get; set; }
            public Guid? Modified_By { get; set; }

        }

        public class Handler : IRequestHandler<UpdateChannelCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateChannelCommand command, CancellationToken cancellationToken)
            {

                var channels = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (channels == null)
                {
                    return Result.Failure(ChannelError.ChannelNotExist());
                }
                else if (channels.ChannelName == command.Channel_Name && channels.SubUnitId == command.SubUnitId)
                {
                    return Result.Failure(ChannelError.ChannelNoChanges());
                }


                var ChannelNameAlreadyExist = await _context.Channels.FirstOrDefaultAsync(x => x.ChannelName == command.Channel_Name, cancellationToken);

                if (ChannelNameAlreadyExist != null && channels.ChannelName != command.Channel_Name)
                {
                    return Result.Failure(ChannelError.ChannelNameAlreadyExist(command.Channel_Name));
                }

                var subUnitNotExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);

                if (subUnitNotExist == null)
                {
                    return Result.Failure(ChannelError.SubUnitNotExist());
                }
                
                var channelInUse = await _context.ChannelUsers.AnyAsync(x => x.ChannelId == command.Id, cancellationToken);

                if(channelInUse == true)
                {
                    return Result.Failure(ChannelError.ChannelInUse(channels.ChannelName));
                }

                channels.ChannelName = command.Channel_Name;
                channels.SubUnitId = command.SubUnitId;
                channels.ModifiedBy = command.Modified_By;
                channels.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);  

                var results = new UpdateChannelResult
                {
                    Id = channels.Id,
                    Channel_Name = channels.ChannelName,
                    SubUnitId = channels.SubUnitId,
                    Modified_By = channels.ModifiedBy,
                    Updated_At = channels.UpdatedAt
                };

                return Result.Success(results);

            }
        }
    }


}
