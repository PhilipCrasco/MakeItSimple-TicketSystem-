using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup
{
    public class AddNewApprover
    {

        public class AddNewApproverCommand : IRequest<Result>
        {
            public int ? SubUnitId { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime CreatedAt { get; set; }
            public Guid? Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

            public IEnumerable<Approver> Approvers { get; set; }

            public class Approver
            {
                public int ? ApproverId { get; set; }
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

                var subUnitNotExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);

                if (subUnitNotExist == null)
                {
                    return Result.Failure(ApproverError.SubUnitNotExist());
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

                    //if (command.Approvers.Count(x => x.ApproverLevel == approver.ApproverLevel) > 1)
                    //{
                    //    return Result.Failure(ApproverError.ApproverLevelDuplicate());
                    //}

                    var approverExist = await _context.Approvers.FirstOrDefaultAsync(x => x.Id == approver.ApproverId, cancellationToken);
                    if (approverExist != null)
                    {

                        var userAlreadyExist = await _context.Approvers.FirstOrDefaultAsync(x => x.SubUnitId == command.SubUnitId && x.UserId == approver.UserId 
                        && approverExist.UserId != approver.UserId, cancellationToken);
                        if (userAlreadyExist != null)
                        {
                            return Result.Failure(ApproverError.UserAlreadyExist());
                        }

                        //var approverLevelDuplicate = await _context.Approvers.FirstOrDefaultAsync(x => x.SubUnitId == command.SubUnitId 
                        //&& x.ApproverLevel == approver.ApproverLevel && approverExist.ApproverLevel != approver.ApproverLevel, cancellationToken);
                        //if(approverLevelDuplicate != null)
                        //{
                        //    return Result.Failure(ApproverError.ApproverLevelDuplicate());
                        //}

                        bool hasChange = false;

                        if(approverExist.SubUnitId != command.SubUnitId)
                        {
                            approverExist.SubUnitId = command.SubUnitId;
                            hasChange = true;
                        }

                        if(approverExist.UserId != approver.UserId)
                        {
                            approverExist.UserId = approver.UserId; 
                            hasChange = true;
                        }
                        if(approverExist.ApproverLevel != approver.ApproverLevel)
                        {
                            approverExist.ApproverLevel = approver.ApproverLevel;
                            hasChange = true;
                        }
                        
                        if(hasChange is true)
                        {
                            approverExist.ModifiedBy = command.Modified_By;
                            approverExist.UpdatedAt = DateTime.Now;
                            approverList.Add(approverExist);
                        }
                        else
                        {
                            approverList.Add(approverExist);
                        }


                    }
                    else
                    {
                        var userAlreadyExist = await _context.Approvers.FirstOrDefaultAsync(x => x.SubUnitId == command.SubUnitId && x.UserId == approver.UserId, cancellationToken);
                        if (userAlreadyExist != null)
                        {
                            return Result.Failure(ApproverError.UserAlreadyExist());
                        }

                        //var approverLevelDuplicate = await _context.Approvers.FirstOrDefaultAsync(x => x.SubUnitId == command.SubUnitId && x.ApproverLevel == approver.ApproverLevel, cancellationToken);
                        //if(approverLevelDuplicate != null)
                        //{
                        //    return Result.Failure(ApproverError.ApproverLevelDuplicate());
                        //}
                        

                        var addApprover = new Approver
                        {
                            UserId = approver.UserId,
                            SubUnitId = command.SubUnitId,
                            ApproverLevel = approver.ApproverLevel,
                            AddedBy = command.Added_By,
                            CreatedAt = DateTime.Now

                        };
                        approverList.Add(addApprover);
                        await _context.Approvers.AddAsync(addApprover, cancellationToken);
                    }

                }

                var removeApproverList = await _context.Approvers.Where(x => x.SubUnitId == subUnitNotExist.Id).ToListAsync();
                var approvalListId = approverList.Select(x => x.Id);

                var removeNotContainApproval = removeApproverList.Where(x => !approvalListId.Contains(x.Id));

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
