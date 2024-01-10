using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup
{
    public class AddNewApprover
    {
        public class AddNewApproverResult
        {
            public int Id { get; set; }
            public int ChannelId { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public Guid? UserId { get; set; }
            public int? ApproverLevel { get; set; }

        }

        public class AddNewApproverCommand : IRequest<Result>
        {
            public int ChannelId { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime CreatedAt { get; set; }

            public IEnumerable<Approver> Approvers { get; set; }

            public class Approver
            {
                public Guid? UserId { get; set; }
                public int? ApproverLevel { get; set; }
            }

        }

        public class Handler : IRequestHandler<AddNewApproverCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewApproverCommand command, CancellationToken cancellationToken)
            {
                var approverList = new List<Approver>();

                var channelNotExist = await _context.Channels.Include(x => x.ChannelUsers).FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);

                if (channelNotExist == null)
                {
                    return Result.Failure(ApproverError.ChannelNotExist());
                }

                var channelAlreadyExist = await _context.Approvers.FirstOrDefaultAsync(x => x.ChannelId == command.ChannelId, cancellationToken);
                if (channelAlreadyExist != null)
                {
                    return Result.Failure(ApproverError.ChannelAlreadyExist());
                }

                foreach (var approver in command.Approvers)
                {

                    if (command.Approvers.Count(x => x.UserId == approver.UserId) > 1)
                    {
                        return Result.Failure(ApproverError.UserDuplicate());
                    }

                    var UserNotExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == approver.UserId, cancellationToken);

                    if (UserNotExist == null)
                    {
                        return Result.Failure(ApproverError.UserNotExist());
                    }

                    var userAlreadyExist = await _context.Approvers.FirstOrDefaultAsync(x => x.UserId == approver.UserId, cancellationToken);
                    if (userAlreadyExist != null)
                    {
                        return Result.Failure(ApproverError.UserAlreadyExist());
                    }

                    var invalidUser = await _context.ChannelUsers.FirstOrDefaultAsync(x => x.ChannelId == command.ChannelId && x.UserId == approver.UserId, cancellationToken);
                    if (invalidUser == null)
                    {
                        return Result.Failure(ApproverError.UserNotAuthorize());
                    }


                    var addApprover = new Approver
                    {
                        UserId = approver.UserId,
                        ChannelId = command.ChannelId,
                        ApproverLevel = approver.ApproverLevel,
                        AddedBy = command.Added_By,
                        CreatedAt = DateTime.Now

                    };
                    approverList.Add(addApprover);
                    await _context.Approvers.AddAsync(addApprover, cancellationToken);


                }

                await _context.SaveChangesAsync(cancellationToken);

                var result = approverList.Select(x => new AddNewApproverResult
                {
                    Id = x.Id,
                    ChannelId = x.ChannelId,
                    Added_By = x.AddedBy,
                    Created_At = x.CreatedAt,
                    UserId = x.UserId,
                    ApproverLevel = x.ApproverLevel,

                }).ToList();

                
               
                return Result.Success(result);
            }
        }

    }
}
