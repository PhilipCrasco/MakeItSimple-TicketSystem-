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
            public List<AddNewReceiverId> AddNewReceiverIds { get; set; }


            public class AddNewReceiverId
            {
                public Guid ? Id { get; set; }
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

                var userNotExist = await _context.Users.Where(x => x.Id == command.UserId
                && x.UserRole.UserRoleName == TicketingConString.Receiver).FirstOrDefaultAsync(cancellationToken);

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

                    if (command.AddNewReceiverIds.Count(x => x.BusinessUnitId == receiverId.BusinessUnitId) > 1)
                    {
                        return Result.Failure(ReceiverError.DuplicateReceiver());
                    }

                    var receiver = await _context.Receivers.FirstOrDefaultAsync(x => x.UserId == receiverId.Id, cancellationToken);
                    var alreadyExist = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == receiverId.BusinessUnitId, cancellationToken);
                    if (receiver == null)
                    {

                        //if (receiver != null)
                        //{

                        //    if (alreadyExist != null && alreadyExist.BusinessUnitId != receiver.BusinessUnitId)
                        //    {
                        //        return Result.Failure(ReceiverError.DuplicateReceiver());
                        //    }

                        //    bool isChange = false;

                        //    if(receiver.UserId != command.UserId)
                        //    {
                        //        receiver.UserId = command.UserId;
                        //        isChange = true;
                        //    }

                        //    if(receiver.BusinessUnitId != receiverId.BusinessUnitId)
                        //    {
                        //        receiver.BusinessUnitId = receiverId.BusinessUnitId;
                        //        isChange = true;

                        //    }

                        //    if (isChange)
                        //    {
                        //        receiver.ModifiedBy = command.Modified_By;
                        //        receiver.UpdatedAt = DateTime.Now;
                        //    }
                        //    else
                        //    {
                        //        receiverList.Add(receiver);
                        //    }

                        //}



                    }
                    else
                    {
                        if (alreadyExist != null)
                        {
                            return Result.Failure(ReceiverError.DuplicateReceiver());
                        }


                        var addNewReceiver = new Receiver
                        {
                            UserId = command.UserId,
                            BusinessUnitId = receiverId.BusinessUnitId,
                            AddedBy = command.Added_By,

                        };

                        receiverList.Add(addNewReceiver);
                        await _context.Receivers.AddAsync(addNewReceiver);
                    }




                }

                var removeApproverList = await _context.Receivers.Where(x => x.UserId == userNotExist.Id).ToListAsync();
                var receiverListId = receiverList.Select(x => x.BusinessUnitId);

                var removeNotContainReceiver = removeApproverList.Where(x => !receiverListId.Contains(x.BusinessUnitId));

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
