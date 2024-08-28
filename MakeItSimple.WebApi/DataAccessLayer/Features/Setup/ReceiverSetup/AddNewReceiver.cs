using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup
{
    public class AddNewReceiver
    {
        public class AddNewReceiverCommand : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }

            public string FullName { get; set; }
            public List<AddNewReceiverId> AddNewReceiverIds { get; set; }


            public class AddNewReceiverId
            {
                public int? ReceiverId { get; set; }
                public int? BusinessUnitId { get; set; }
            }

        }


        public class Handler : IRequestHandler<AddNewReceiverCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewReceiverCommand command, CancellationToken cancellationToken)
            {

                var receiverList = new List<Receiver>();

                var allUserList = await _context.UserRoles
                    .AsNoTracking()
                    .ToListAsync();

                var receiverPermissionList = allUserList
                    .Where(x => x.Permissions
                .Contains(TicketingConString.Receiver))
                    .Select(x => x.Id).ToList();

                var userNotExist = await _context.Users
                    .Where(x => x.Id == command.UserId
                && receiverPermissionList.Contains(x.UserRoleId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (userNotExist == null)
                {
                    return Result.Failure(ReceiverError.UserUnitNotExist());
                }

                foreach (var receiverId in command.AddNewReceiverIds)
                {
                    var businessNotExist = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == receiverId.BusinessUnitId, cancellationToken);
                    if (businessNotExist == null)
                    {
                        return Result.Failure(ReceiverError.BusinessUnitNotExist());
                    }


                    var receiverUser = await _context.Receivers.FirstOrDefaultAsync(x => x.User.Fullname == command.FullName, cancellationToken);
                    var alreadyExist = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == receiverId.BusinessUnitId, cancellationToken);
                    if (receiverUser != null)
                    {

                        var receiver = await _context.Receivers.FirstOrDefaultAsync(x => x.Id == receiverId.ReceiverId, cancellationToken);
                        if (receiver != null)
                        {

                            if (alreadyExist != null && alreadyExist.BusinessUnitId != receiver.BusinessUnitId)
                            {
                                return Result.Failure(ReceiverError.DuplicateReceiver());
                            }

                            bool isChange = false;

                            if (receiver.UserId != command.UserId)
                            {
                                receiver.UserId = command.UserId;
                                isChange = true;
                            }

                            if (receiver.BusinessUnitId != receiverId.BusinessUnitId)
                            {
                                receiver.BusinessUnitId = receiverId.BusinessUnitId;
                                isChange = true;
                            }

                            if (isChange)
                            {
                                receiver.ModifiedBy = command.Modified_By;
                                receiver.UpdatedAt = DateTime.Now;
                                receiverList.Add(receiver);
                            }
                            else
                            {
                                receiverList.Add(receiver);

                            }

                        }
                        else
                        {

                            var addNewReceiver = new Receiver
                            {
                                UserId = command.UserId,
                                BusinessUnitId = receiverId.BusinessUnitId,
                                AddedBy = command.Added_By,

                            };


                            await _context.Receivers.AddAsync(addNewReceiver);
                            receiverList.Add(addNewReceiver);

                        }


                    }
                    else
                    {

                        var addNewReceiver = new Receiver
                        {
                            UserId = command.UserId,
                            BusinessUnitId = receiverId.BusinessUnitId,
                            AddedBy = command.Added_By,

                        };


                        await _context.Receivers.AddAsync(addNewReceiver);
                        receiverList.Add(addNewReceiver);

                    }

                }

                var removeApproverList = await _context.Receivers.Where(x => x.UserId == userNotExist.Id).ToListAsync();
                var receiverListId = receiverList.Select(x => x.Id);

                var removeNotContainReceiver = removeApproverList.Where(x => !receiverListId.Contains(x.Id));

                foreach (var receiverRemove in removeNotContainReceiver)
                {
                    _context.Remove(receiverRemove);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }

    }
}