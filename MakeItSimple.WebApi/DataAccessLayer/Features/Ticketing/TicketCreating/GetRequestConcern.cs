using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class GetRequestConcern
    {
        public class GetRequestConcernResult
        {
            public int? RequestTransactionId { get; set; }
            public int ? DepartmentId { get; set; }
            public string Department_Name { get; set; }
            public string Concern_Details { get; set; }
            public Guid? Requestor_By { get; set; }
            public string Requestor_Name { get; set; }
            public int ? ProjectId { get; set; }
            public string Project_Name { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public int? UnitId { get; set; }
            public string Unit_Name { get; set; }
            public int? SubUnitId { get; set; }
            public string SubUnit_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Issue_Handler { get; set; }
            public string Ticket_Status { get; set; }
            //public string Concern_Status {  get; set; }
            public string Remarks { get; set; }
            public string Concern_Type { get; set; }
            public bool ? Done { get; set; }
            public bool ? Is_Assigned { get; set; }
            public List<GetRequestConcernByConcern> GetRequestConcernByConcerns { get; set; }

            public class GetRequestConcernByConcern
            {

                public int? TicketConcernId { get; set; }
                public string Ticket_No { get; set; }
                public string Concern_Description { get; set; }
                public string Category_Description { get; set; }
                public string SubCategory_Description { get; set; }
                public DateTime ? Start_Date { get; set; }
                public DateTime ? Target_Date { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }
                public bool IsActive { get; set; }


            }

            public class GetRequestConcernQuery : UserParams, IRequest< PagedList<GetRequestConcernResult>>
            {
                public Guid? UserId { get; set; }
                public string Role { get; set; }
                public string UserType { get; set; }
                public bool ? Approval { get; set; }
                public string Search { get; set; }
                public bool ? Status { get; set; }
                public bool ? Reject { get; set; }
                public bool? Ascending { get; set; }
                

            }
            public class Handler : IRequestHandler<GetRequestConcernQuery, PagedList<GetRequestConcernResult>>
            {
                private readonly MisDbContext _context;

                public Handler(MisDbContext context)
                {
                    _context = context;
                }

                public async Task<PagedList<GetRequestConcernResult>> Handle(GetRequestConcernQuery request, CancellationToken cancellationToken)
                {
                    IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
                        .Include(x => x.ModifiedByUser)  
                        .Include(x => x.AddedByUser)
                        .Include(x => x.Channel)
                        .ThenInclude(x  => x.Project)
                        .Include(x => x.User)
                        .Include(x => x.RequestorByUser)
                        .ThenInclude(x => x.UserRole)
                        .Include(x => x.RequestConcern); 


                    if(ticketConcernQuery.Count() > 0)
                    {
                        var businessUnitList = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == ticketConcernQuery.First().RequestorByUser.BusinessUnitId);
                        var receiverList = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                        var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                        var fillterApproval = ticketConcernQuery.Select(x => x.RequestTransactionId);

                        var allUserList = await _context.UserRoles.ToListAsync();

                        var receiverPermissionList = allUserList.Where(x => x.Permissions
                        .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                        var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                        .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();

                        var requestorPermissionList = allUserList.Where(x => x.Permissions
                        .Contains(TicketingConString.Requestor)).Select(x => x.UserRoleName).ToList();

                        var approverPermissionList = allUserList.Where(x => x.Permissions
                        .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();


                        if (!string.IsNullOrEmpty(request.Search))
                        {
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestorByUser.Fullname.Contains(request.Search));
                        }

                        if(request.Ascending != null)
                        {
                            if(request.Ascending is true)
                            {
                                ticketConcernQuery = ticketConcernQuery.OrderBy(x => x.RequestTransactionId);
                            }
                            else
                            {
                                ticketConcernQuery = ticketConcernQuery.OrderByDescending(x => x.RequestTransactionId);
                            }

                        }

                        if (request.Status != null)
                        {
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == request.Status);
                        }

                        if (request.Reject != null)
                        {
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.IsReject == request.Reject);
                        }


                        if (request.Approval != null)
                        {
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.IsApprove == request.Approval);
                        }


                        if(!string.IsNullOrEmpty(request.UserType))
                        {
                            if (request.UserType == TicketingConString.Approver)
                            {
                                if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList != null)
                                {
                                    if (request.UserId == receiverList.UserId)
                                    {
                                        var approverTransactList = await _context.ApproverTicketings
                                            .Where(x => fillterApproval.Contains(x.RequestTransactionId) && x.IsApprove != true)
                                            .ToListAsync();

                                        if (approverTransactList != null && approverTransactList.Any())
                                        {
                                            var generatedIdInApprovalList = approverTransactList.Select(approval => approval.RequestTransactionId);
                                            ticketConcernQuery = ticketConcernQuery.Where(x => !generatedIdInApprovalList.Contains(x.RequestTransactionId));
                                        }
                                        var receiver = await _context.TicketConcerns.Include(x => x.RequestorByUser)
                                            .Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId)
                                            .ToListAsync();

                                        var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);
                                        var requestorSelect = receiver.Select(x => x.RequestTransactionId);

                                        ticketConcernQuery = ticketConcernQuery
                                            .Where(x => receiverContains.Contains(x.RequestorByUser.BusinessUnitId) 
                                            
                                            && requestorSelect.Contains(x.RequestTransactionId) 
                                            && x.RequestorByUser.UserRole.UserRoleName != TicketingConString.Requestor);
                                    }
                                    else
                                    {
                                        ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestTransactionId == null);
                                    }

                                }

                                else if (approverPermissionList.Any(x => x.Contains(request.Role)))
                                {
                                    var approverTransactList = await _context.ApproverTicketings
                                        .Where(x => x.UserId == userApprover.Id).ToListAsync();

                                    var approvalLevelList = approverTransactList
                                        .Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove != true)
                                        .ToList();

                                    var userRequestIdApprovalList = approvalLevelList.Select(x => x.RequestTransactionId);

                                    var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                                    ticketConcernQuery = ticketConcernQuery
                                        .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                                        && userRequestIdApprovalList.Contains(x.RequestTransactionId));

                                }
                                else
                                {
                                    ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestTransactionId == null);
                                }

                                if (request.UserType == TicketingConString.Requestor)
                                {
                                     if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                                    {
                                        var requestConcernList = await _context.RequestConcerns.Where(x => x.UserId == request.UserId).ToListAsync();
                                        var requestConcernContains = requestConcernList.Select(x => x.UserId);
                                        ticketConcernQuery = ticketConcernQuery.Where(x => requestConcernContains.Contains(x.RequestorBy));
                                    }
                                    else
                                    {
                                        ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestTransactionId == null);
                                    }

                                }

                                if (request.UserType == TicketingConString.IssueHandler)
                                {
                                    if (issueHandlerPermissionList.Any(x => x.Contains(request.Role)))
                                    {
                                        ticketConcernQuery = ticketConcernQuery.Where(x => x.UserId == request.UserId);
                                    }
                                    else
                                    {
                                        ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestTransactionId == null);
                                    }

                                }

                            }

                        }

                    }

                    var result = ticketConcernQuery
                        //.Where(x => x.RequestorByUser.UserRole.UserRoleName != TicketingConString.Requestor)
                        .GroupBy(x => new
                    {

                        x.RequestTransactionId,
                        x.UserId,
                        IssueHandler = x.User.Fullname
                        
                    }).Select(x => new GetRequestConcernResult
                    {

                        RequestTransactionId = x.Key.RequestTransactionId,
                        UserId = x.Key.UserId,
                        Issue_Handler = x.Key.IssueHandler,
                        DepartmentId = x.First().RequestorByUser.DepartmentId,
                        Department_Name = x.First().RequestorByUser.Department.DepartmentName,
                        Concern_Details = x.First().RequestConcern.Concern,
                        UnitId = x.First().User.UnitId,
                        Unit_Name = x.First().User.Units.UnitName,
                        SubUnitId = x.First().User.SubUnitId,
                        SubUnit_Name = x.First().User.SubUnit.SubUnitName,
                        ProjectId = x.First().Channel.ProjectId,
                        Project_Name = x.First().Channel.Project.ProjectName,
                        ChannelId = x.First().ChannelId,
                        Channel_Name = x.First().Channel.ChannelName,
                        Requestor_By = x.First().RequestorBy,
                        Requestor_Name = x.First().RequestorByUser.Fullname,
                        Ticket_Status = x.First().IsApprove == true ? "Ticket Approve" : x.First().IsReject == true ? "Rejected" :
                         x.First().ConcernStatus != TicketingConString.ForApprovalTicket ? x.First().ConcernStatus : x.First().IsApprove == false 
                         && x.First().IsReject == false ? "For Approval" : "Unknown" ,

                        Remarks = x.First().Remarks,
                        Concern_Type = x.First().TicketType,
                        //Concern_Status = x.First().ConcernStatus,
                        Done = x.First().IsDone,
                        Is_Assigned = x.First().IsAssigned,
                        GetRequestConcernByConcerns = x.Select(x => new GetRequestConcernResult.GetRequestConcernByConcern
                        {
                            TicketConcernId = x.Id,
                            Ticket_No = x.TicketNo,
                            Concern_Description = x.ConcernDetails,
                            Category_Description = x.Category.CategoryDescription,
                            SubCategory_Description = x.SubCategory.SubCategoryDescription,
                            Start_Date = x.StartDate,
                            Target_Date = x.TargetDate,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Modified_By = x.ModifiedByUser.Fullname,
                            Updated_At = x.UpdatedAt,
                            IsActive = x.IsActive,

                        }).ToList()

                    });

                    return await PagedList<GetRequestConcernResult>.CreateAsync(result, request.PageNumber, request.PageSize);

                }



            }

        }
    }
}
