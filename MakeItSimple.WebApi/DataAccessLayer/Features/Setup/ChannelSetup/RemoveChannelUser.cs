using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class RemoveChannelUser
    {

        public class RemoveChannelUserResult
        {
            public int ChannelId { get; set; }
            public Guid ? Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }

            public ICollection<ChannelUser> ChannelUsers {  get; set; } 

            public class ChannelUser
            {
                public Guid UserId { get; set; }
            }

        }

        public class RemoveChannelUserCommand : IRequest<Result>
        {

            public int ChannelId { get; set; }
            public Guid? Modified_By { get; set; }

            public List<ChannelUser> ChannelUsers { get; set; }

            public class ChannelUser
            {
                public Guid UserId { get; set; }
            }
        }


        public class Handler : IRequestHandler<RemoveChannelUserCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RemoveChannelUserCommand command, CancellationToken cancellationToken)
            {
                var channelUsers = await _context.ChannelUsers
                    .Include(ch => ch.Channel)
                    .Where(x => x.ChannelId == command.ChannelId)
                    .ToListAsync();

                var resultList = new List<RemoveChannelUserResult>();


                if(channelUsers == null)
                {
                    return Result.Failure(ChannelError.ChannelUserNotExist());
                }


        
                foreach (var channelUser in command.ChannelUsers)
                {
                    var cu = channelUsers.FirstOrDefault(x => x.UserId == channelUser.UserId);

                    if (cu == null) 
                    {
                        return Result.Failure(ChannelError.ChannelUserNotExist());
                    }

                    cu.Channel.UpdatedAt = DateTime.Now;
                    cu.Channel.ModifiedBy = command.Modified_By;

                    resultList.Add(new RemoveChannelUserResult
                    {
                        ChannelId = cu.ChannelId,
                        Updated_At = DateTime.Now,
                        Modified_By = command.Modified_By,
                        ChannelUsers = command.ChannelUsers.Select(x => new RemoveChannelUserResult.ChannelUser
                        {
                            UserId = x.UserId

                        }).ToList()
                    });

                    _context.ChannelUsers.Remove(cu);

                }

                await _context.SaveChangesAsync(cancellationToken);


                return Result.Success(resultList);
            }
        }
    }
}
