using MediatR;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MakeItSimple.WebApi.Common;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.AddNewChannel.AddNewChannelCommands;
using MakeItSimple.WebApi.Models;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class AddNewChannel
    {
        public class AddNewChannelCommands : IRequest<Result>
        {
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }

            public List<ChannelUserById> ChannelUserByIds { get; set; }
            public class ChannelUserById
            {
                public int? ChannelUserId { get; set; }
                public Guid? UserId { get; set; }

            }

        }


        public class Handler : IRequestHandler<AddNewChannelCommands, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }



            public async Task<Result> Handle(AddNewChannelCommands command, CancellationToken cancellationToken)
            {

                var listDelete = new List<ChannelUser>();
                var listChannel = new List<Channel>();

                var channelId = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);


                if (channelId != null)
                {
                    var channelNameAlreadyExist = await _context.Channels.FirstOrDefaultAsync(x => x.ChannelName == command.Channel_Name, cancellationToken);

                    if (channelNameAlreadyExist != null && channelNameAlreadyExist.ChannelName != channelId.ChannelName)
                    {
                        return Result.Failure(ChannelError.ChannelNameAlreadyExist(command.Channel_Name));
                    }

                    bool hasChange = false;

                    if (channelId.ChannelName != command.Channel_Name)
                    {
                        channelId.ChannelName = channelId.ChannelName;
                        hasChange = true;
                    }


                    if (hasChange)
                    {
                        channelId.ModifiedBy = command.Modified_By;
                        channelId.UpdatedAt = DateTime.Now;

                    }


                    listChannel.Add(channelId);


                }
                else
                {

                    var channelNameAlreadyExist = await _context.Channels.FirstOrDefaultAsync(x => x.ChannelName == command.Channel_Name, cancellationToken);

                    if (channelNameAlreadyExist != null)
                    {
                        return Result.Failure(ChannelError.ChannelNameAlreadyExist(command.Channel_Name));
                    }

                    var channels = new Channel
                    {
                        ChannelName = command.Channel_Name,
                        AddedBy = command.Added_By,

                    };

                    await _context.Channels.AddAsync(channels, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    listChannel.Add(channels);
                }

                foreach (var member in command.ChannelUserByIds)
                {

                    var UserNotExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == member.UserId, cancellationToken);

                    if (UserNotExist == null)
                    {
                        return Result.Failure(ChannelError.UserNotExist());
                    }



                    var channelName = await _context.Channels.FirstOrDefaultAsync(x => x.ChannelName == command.Channel_Name, cancellationToken);

                    var channelUser = await _context.ChannelUsers.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == member.UserId && x.ChannelId == channelName.Id);

                    if (channelUser == null)
                    {
                        var addChannelUser = new ChannelUser
                        {
                            ChannelId = channelName.Id,
                            UserId = member.UserId,

                        };

                        await _context.ChannelUsers.AddAsync(addChannelUser, cancellationToken);
                        listDelete.Add(addChannelUser);
                    }
                    else
                    {
                        listDelete.Add(channelUser);
                    }


                }

                var channelList = listChannel.Select(x => x.Id);
                var removeApproverList = await _context.ChannelUsers.Where(x => channelList.Contains(x.ChannelId)).ToListAsync();
                var approvalListId = listDelete.Select(x => x.UserId);

                var removeNotContainApproval = removeApproverList.Where(x => !approvalListId.Contains(x.UserId));

                foreach (var approver in removeNotContainApproval)
                {
                    _context.Remove(approver);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();

            }
        }
    }
}