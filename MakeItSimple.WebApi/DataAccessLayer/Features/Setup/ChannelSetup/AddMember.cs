using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class AddMember
    {

       public class AddMemberCommand : IRequest<Result>
        {
            public int ChannelId { get; set; }
            public List<Member> Members { get; set; }
            public class Member
            {
               public Guid? UserId { get; set; }
            }


        }


        public class Handler : IRequestHandler<AddMemberCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddMemberCommand command, CancellationToken cancellationToken)
            {

                var ChannelNotExist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);

                if (ChannelNotExist == null)
                {
                    return Result.Failure(ChannelError.ChannelNotExist());
                }

                foreach(var member in command.Members)
                {

                    var UserNotExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == member.UserId, cancellationToken);

                    if (ChannelNotExist == null)
                    {
                        return Result.Failure(ChannelError.UserNotExist());
                    }


                    var UserAlreadyAdd = await _context.ChannelUsers.FirstOrDefaultAsync(x => x.UserId == member.UserId
                    && x.ChannelId == command.ChannelId, cancellationToken);

                    if (UserAlreadyAdd != null)
                    {
                        return Result.Failure(ChannelError.UserAlreadyAdd());
                    }

                    var channelUser = new ChannelUser
                    {
                        ChannelId = command.ChannelId,
                        UserId = member.UserId,

                    };

                    await _context.ChannelUsers.AddAsync(channelUser, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }


        }



    }

}
