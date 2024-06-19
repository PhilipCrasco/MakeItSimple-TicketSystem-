using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransferTicket;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate
{
    public class GetReDate
    {
        public class GetReDateResult
        {

            public int? TicketConcernId { get; set; }
            public int? TicketReDateId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }
            public string Concern_Details { get; set; }
            public string Category_Description { get; set; }
            public string SubCategory_Description { get; set; }
            public DateTime? Start_Date { get; set; }
            public DateTime? Target_Date { get; set; }
            public bool IsActive { get; set; }
            public string ReDate_By { get; set; }
            public DateTime? ReDate_At { get; set; }
            public string ReDate_Status { get; set; }
            public string ReDate_Remarks { get; set; }
            public string RejectReDate_By { get; set; }
            public DateTime? RejectReDate_At { get; set; }
            public string Reject_Remarks { get; set; }
            public string Remarks { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

        }

        public class GetReDateQuery : UserParams, IRequest<PagedList<GetReDateResult>>
        {
            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public bool? IsReDate { get; set; }
            public bool? IsReject { get; set; }
            public string Search { get; set; }
            public bool? Status { get; set; }

        }

        public class Handler : IRequestHandler<GetReDateQuery, PagedList<GetReDateResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetReDateResult>> Handle(GetReDateQuery request, CancellationToken cancellationToken)
            {
                IQueryable<TicketReDate> ticketReDateQuery = _context.TicketReDates
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .ThenInclude(x => x.Department)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.Channel)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.SubCategories)
                    .Include(x => x.RequestTransaction)
                    .ThenInclude(x => x.ApproverTicketings)
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.ReDateByUser)
                    .Include(x => x.ReDateByUser);
                    
                

                var allUserList = await _context.UserRoles.ToListAsync();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();


                var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();

                if (!string.IsNullOrEmpty(request.Search))
                {
                    ticketReDateQuery = ticketReDateQuery
                        .Where(x => x.TicketConcern.User.Fullname.Contains(request.Search)
                    || x.TicketConcernId.ToString().Contains(request.Search));
                }

                if (request.Status != null)
                {
                    ticketReDateQuery = ticketReDateQuery.Where(x => x.IsActive == request.Status);
                }

                if (request.IsReDate != null)
                {

                    ticketReDateQuery = ticketReDateQuery.Where(x => x.IsReDate == request.IsReDate);
                }


                if (request.IsReject != null)
                {
                    ticketReDateQuery = ticketReDateQuery.Where(x => x.IsRejectReDate == request.IsReject);
                }

                if (!string.IsNullOrEmpty(request.UserType))
                {
                    if (request.UserType == TicketingConString.Approver)
                    {

                        if (approverPermissionList.Any(x => x.Contains(request.Role)))
                        {

                            var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                            var approverTransactList = await _context.ApproverTicketings
                                .Where(x => x.UserId == userApprover.Id)
                                .ToListAsync();

                            var approvalLevelList = approverTransactList
                                .Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null)
                                .ToList();

                            var userRequestIdApprovalList = approvalLevelList.Select(x => x.TicketReDateId);

                            var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);

                            ticketReDateQuery = ticketReDateQuery
                                .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                                && userRequestIdApprovalList.Contains(x.Id));
                        }
                        else
                        {
                            return new PagedList<GetReDateResult>(new List<GetReDateResult>(), 0, request.PageNumber, request.PageSize);
                        }

                    }
                    else if (request.UserType == TicketingConString.IssueHandler)
                    {
                        ticketReDateQuery = ticketReDateQuery.Where(x => x.AddedByUser.Id == request.UserId);
                    }
                    else
                    {
                        return new PagedList<GetReDateResult>(new List<GetReDateResult>(), 0, request.PageNumber, request.PageSize);
                    }
                }


                var results = ticketReDateQuery.Select(x => new GetReDateResult
                {

                    TicketConcernId = x.TicketConcernId,
                    TicketReDateId = x.Id,
                    Department_Code = x.TicketConcern.User.Department.DepartmentCode,
                    Department_Name = x.TicketConcern.User.Department.DepartmentName,
                    ChannelId = x.TicketConcern.ChannelId,
                    Channel_Name = x.TicketConcern.Channel.ChannelName,
                    UserId = x.TicketConcern.UserId,
                    Fullname = x.TicketConcern.User.Fullname,
                    Concern_Details = x.TicketConcern.ConcernDetails,
                    Category_Description = x.TicketConcern.Category.CategoryDescription,
                    SubCategory_Description = x.TicketConcern.SubCategory.SubCategoryDescription,
                    Start_Date = x.StartDate,
                    Target_Date = x.TargetDate,
                    IsActive = x.IsActive,
                    ReDate_By = x.ReDateByUser.Fullname,
                    ReDate_At = x.ReDateAt,
                    ReDate_Status = x.IsReDate == false && x.IsRejectReDate == false ? "For Re-Date Approval"
                                    : x.IsReDate == true && x.IsRejectReDate == false ? "Re-Date Approve"
                                    : x.IsRejectReDate == true ? "Re-Date Reject" : "Unknown",
                    ReDate_Remarks = x.ReDateRemarks,
                    RejectReDate_By = x.RejectReDateByUser.Fullname,
                    RejectReDate_At = x.RejectReDateAt,
                    Reject_Remarks = x.RejectRemarks,
                    Remarks = x.Remarks,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt

                });


                return await PagedList<GetReDateResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
