using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.GetReTicket.GetReTicketResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class GetReTicket
    {
        public class GetReTicketResult
        {
            public int? RequestGeneratorId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Channel_Name { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public bool IsActive { get; set; }
            public string Re_Ticket_By { get; set; }
            public DateTime? Re_Ticket_At { get; set; }
            public string Re_Ticket_Status { get; set; }
            public string Re_Ticket_Remarks { get; set; }
            public string RejectReTicket_By { get; set; }
            public DateTime? RejectReTicket_At { get; set; }
            public string Reject_Remarks { get; set; }

            public List<GetTransferConcern> GetTransferConcerns { get; set; }
            public class GetTransferConcern
            {
                public int ReTicketConcernId { get; set; }
                public int TicketConcernId { get; set; }
                public string Concern_Details { get; set; }
                public string Category_Description { get; set; }
                public string SubCategoryDescription { get; set; }

                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }

                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }

                public DateTime? Start_Date { get; set; }
                public DateTime? Target_Date { get; set; }
            }

        }

        public class GetReTicketQuery : UserParams, IRequest<PagedList<GetReTicketResult>>
        {
            public Guid ? UserId { get; set; }
            public string Approval { get; set; }
            public string Users { get; set; }
            public string Role { get; set; }

            public string Search { get; set; }
            public bool ? IsReTicket { get; set; }
            public bool ? IsReject { get ; set; }
            public Guid? UserApproverId { get; set; }

        }

        public class Handler : IRequestHandler<GetReTicketQuery, PagedList<GetReTicketResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetReTicketResult>> Handle(GetReTicketQuery request, CancellationToken cancellationToken)
            {
                IQueryable<ReTicketConcern> reTicketQuery = _context.ReTicketConcerns
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.Department)
                    .Include(x => x.SubUnit)
                    .Include(x => x.Channel)
                    .Include(x => x.User)
                    .Include(x => x.RejectReTicketByUser)
                    .Include(x => x.ReTicketByUser);

                //var channeluserExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);


                var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserApproverId, cancellationToken);
                var fillterApproval = reTicketQuery.Select(x => x.RequestGeneratorId);

                if (TicketingConString.Approval == request.Approval)
                {

                    if (request.UserId != null && TicketingConString.Approver == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => x.UserId == userApprover.Id).ToListAsync();
                        var approvalLevelList = approverTransactList.Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null).ToList();
                        var userRequestIdApprovalList = approvalLevelList.Select(x => x.RequestGeneratorId);
                        var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                        reTicketQuery = reTicketQuery.Where(x => userIdsInApprovalList.Contains(x.TicketApprover) && userRequestIdApprovalList.Contains(x.RequestGeneratorId));

                    }

                    else if (TicketingConString.Admin == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => fillterApproval.Contains(x.RequestGeneratorId) && x.IsApprove == null).ToListAsync();

                        if (approverTransactList != null && approverTransactList.Any())
                        {
                            var generatedIdInApprovalList = approverTransactList.Select(approval => approval.RequestGeneratorId);
                            reTicketQuery = reTicketQuery.Where(x => !generatedIdInApprovalList.Contains(x.RequestGeneratorId));
                        }

                    }
                    else
                    {
                        reTicketQuery = reTicketQuery.Where(x => x.RequestGeneratorId == null);
                    }

                }

                if (TicketingConString.Users == request.Users)
                {
                    var channelUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                    if (channelUser != null)
                    {
                        
                       reTicketQuery = reTicketQuery.Where(x => x.AddedByUser.Id == request.UserId);
                        
                    }
                }

                
                if(!string.IsNullOrEmpty(request.Search))
                {
                    reTicketQuery = reTicketQuery.Where(x => x.User.Fullname.Contains(request.Search)
                                    || x.User.EmpId.Contains(request.Search));
                }

                if(request.IsReject != null)
                {
                    reTicketQuery = reTicketQuery.Where(x => x.IsReTicket == request.IsReject);
                }

                if (request.IsReTicket != null)
                {
                    reTicketQuery = reTicketQuery.Where(x => x.IsReTicket == request.IsReTicket);
                }


                var results = reTicketQuery.GroupBy(x => x.RequestGeneratorId)
                    .Select(x => new GetReTicketResult
                    {
                        RequestGeneratorId = x.Key, 
                        Department_Code = x.First().Department.DepartmentCode,
                        Department_Name = x.First().Department.DepartmentName,
                        SubUnit_Code = x.First().SubUnit.SubUnitCode,
                        SubUnit_Name = x.First().SubUnit.SubUnitName   ,
                        Channel_Name = x.First().Channel.ChannelName,
                        EmpId = x.First().User.EmpId,   
                        Fullname = x.First().User.Fullname,
                        IsActive = x.First().User.IsActive,
                        RejectReTicket_By = x.First().RejectReTicketByUser.Fullname,
                        RejectReTicket_At = x.First().ReTicketAt,
                        Reject_Remarks = x.First().RejectRemarks,
                        Re_Ticket_By = x.First().ReTicketByUser.Fullname,
                        Re_Ticket_At = x.First().ReTicketAt,
                        Re_Ticket_Remarks = x.First().ReTicketRemarks,
                        GetTransferConcerns = x.Select(x => new GetReTicketResult.GetTransferConcern
                        {
                            ReTicketConcernId = x.Id,
                            TicketConcernId = x.TicketConcernId,
                            Concern_Details = x.ConcernDetails,
                            Category_Description = x.Category.CategoryDescription,
                            SubCategoryDescription = x.SubCategory.SubCategoryDescription,
                            Start_Date = x.StartDate,
                            Target_Date = x.TargetDate,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Updated_At = x.UpdatedAt,
                            Modified_By = x.ModifiedByUser.Fullname

                        }).ToList(),
                    });

                return await PagedList<GetReTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
