using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetRequestorTicketConcern.GetRequestorTicketConcernResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class GetRequestorTicketConcern
    {
        public class GetRequestorTicketConcernResult
        {
            public int? RequestTransactionId { get; set; }

            public int? DepartmentId { get; set; }
            public string Department_Name { get; set; }
            public Guid? UserId { get; set; }



            public string EmpId { get; set; }

            public string FullName { get; set; }
            public string Concern { get; set; }

            public int? RequestConcernId { get; set; }

            public string Concern_Status { get; set; }
            public bool? Is_Done { get; set; }
            public string Remarks { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? updated_At { get; set; }

            public List<TicketRequestConcern> TicketRequestConcerns { get; set; }
            public class TicketRequestConcern
            {
                public int? RequestTransactionId { get; set; }
                public int? DepartmentId { get; set; }
                public string Department_Name { get; set; }
                public int? ChannelId { get; set; }
                public string Channel_Name { get; set; }
                public int? UnitId { get; set; }
                public string Unit_Name { get; set; }
                public int? SubUnitId { get; set; }
                public string SubUnit_Name { get; set; }
                public string Concern_Description { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
                public DateTime? Start_Date { get; set; }
                public DateTime? Target_Date { get; set; }
                public string Ticket_Status { get; set; }
                public bool ? Is_Assigned { get; set; }
                //public string Concern_Status {  get; set; }
                public string Remarks { get; set; }
                public string Concern_Type { get; set; }
                public List<TicketConcern> TicketConcerns { get; set; }
                public class TicketConcern
                {
                    public int? TicketConcernId { get; set; }
                    //public int ? ChannelUserId { get; set; }
                    
                    public Guid? UserId { get; set; }
                    public int? SubUnitId { get; set; }
                    public string SubUnit_Name { get; set; }
                    public string Issue_Handler { get; set; }
                    public string Added_By { get; set; }
                    public DateTime Created_At { get; set; }
                    public string Modified_By { get; set; }
                    public DateTime? Updated_At { get; set; }
                    public bool IsActive { get; set; }
                }
            }

        }

        public class GetRequestorTicketConcernQuery : UserParams, IRequest<PagedList<GetRequestorTicketConcernResult>>
        {
            //public string Requestor { get; set; }
            //public string Approver { get; set; }
            public string UserType {  get; set; }

            public string Role { get; set; }
            public Guid? UserId { get; set; }
            public string Concern_Status { get; set; }
            public string Search { get; set; }
            public bool? Status { get; set; }

            public bool? Ascending { get; set; }
        }

        public class Handler : IRequestHandler<GetRequestorTicketConcernQuery, PagedList<GetRequestorTicketConcernResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetRequestorTicketConcernResult>> Handle(GetRequestorTicketConcernQuery request, CancellationToken cancellationToken)
            {
                    var requestConcernsQuery = _context.RequestConcerns
                    .Include(x => x.User)
                     .Include(x => x.AddedByUser)
                     .Include(x => x.ModifiedByUser)
                     .Include(x => x.User)
                     .ThenInclude(x => x.Department)
                     .Include(x => x.TicketConcerns)
                     .ThenInclude(x => x.User)
                     .Include(x => x.TicketConcerns)
                     .ThenInclude(x => x.RequestorByUser)
                     .Include(x => x.TicketConcerns)
                     .ThenInclude(x => x.Channel)
                     .ThenInclude(x => x.ChannelUsers)
                     .AsQueryable();

                if (requestConcernsQuery.Any())
                {
                    var businessUnitList = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == requestConcernsQuery.First().User.BusinessUnitId);
                    var receiverList = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                    var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                    var fillterApproval = requestConcernsQuery.Select(x => x.RequestTransactionId);

                    var allUserList = await _context.UserRoles.ToListAsync();

                    var receiverPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                    var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();

                    var requestorPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor)).Select(x => x.UserRoleName).ToList();

                    var approverPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                    if (request.Status != null)
                    {
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.IsActive == request.Status);
                    }

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.User.Fullname.Contains(request.Search) || x.RequestTransactionId.ToString().Contains(request.Search));
                    }

                    if (request.Ascending != null)
                    {
                        requestConcernsQuery = request.Ascending.Value
                            ? requestConcernsQuery.OrderBy(x => x.RequestTransactionId)
                            : requestConcernsQuery.OrderByDescending(x => x.RequestTransactionId);
                    }

                    if (request.Concern_Status != null)
                    {
                        switch (request.Concern_Status)
                        {
                            case TicketingConString.Approval:
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket);
                                break;
                            case TicketingConString.OnGoing:
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing);
                                break;
                            case TicketingConString.Done:
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.Done);
                                break;
                            default:
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestTransactionId == null);
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        if (request.UserType == TicketingConString.Requestor)
                        {
                            if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                            {
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.UserId == request.UserId);
                            }
                            else
                            {
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestTransactionId == null);
                            }

                        }

                        if (request.UserType == TicketingConString.Approver)
                        {
                            if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList != null)
                            {
                                if (request.UserId == receiverList.UserId)
                                {
                                    //var ticketConcernApproveList = await _context.

                                    var receiver = await _context.TicketConcerns.Include(x => x.RequestorByUser)
                                        .Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId && x.IsApprove != true)
                                        .ToListAsync();
                                    var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);
                                    var requestorSelect = receiver.Select(x => x.RequestTransactionId);

                                    requestConcernsQuery = requestConcernsQuery
                                        .Where(x => receiverContains.Contains(x.User.BusinessUnitId) && requestorSelect
                                             .Contains(x.RequestTransactionId));
                                }
                                else
                                {
                                    requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestTransactionId == null);
                                }

                            }
                            else
                            {
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestTransactionId == null);
                            }
                        }

                    }

                }

                var results =  requestConcernsQuery
                    .GroupBy(x => new
                    {
                        x.RequestTransactionId,
                        x.User.DepartmentId,
                        Department_Name = x.User.Department.DepartmentName,
                        x.UserId,
                        x.User.EmpId,
                        FullName = x.User.Fullname,
                        x.Concern,
                        x.Id,
                        x.ConcernStatus,
                        x.IsDone,
                        x.Remarks,
                        Added_By = x.AddedByUser.Fullname,
                        x.CreatedAt,
                        Modified_By = x.ModifiedByUser.Fullname,
                        x.UpdatedAt
                    })
                    .Select(g => new GetRequestorTicketConcernResult
                    {
                        RequestTransactionId = g.Key.RequestTransactionId,
                        DepartmentId = g.Key.DepartmentId,
                        Department_Name = g.Key.Department_Name,
                        UserId = g.Key.UserId,
                        EmpId = g.Key.EmpId,
                        FullName = g.Key.FullName,
                        Concern = g.Key.Concern,
                        RequestConcernId = g.Key.Id,
                        Concern_Status = g.Key.ConcernStatus,
                        Is_Done = g.Key.IsDone,
                        Remarks = g.Key.Remarks,
                        Added_By = g.Key.Added_By,
                        Created_At = g.Key.CreatedAt,
                        Modified_By = g.Key.Modified_By,
                        updated_At = g.Key.UpdatedAt,
                        TicketRequestConcerns = g.SelectMany(tc => tc.TicketConcerns)
                            .GroupBy(tc => new
                            {
                                tc.RequestTransactionId,
                                DepartmentId = tc.User.DepartmentId,
                                Department_Name = tc.User.Department.DepartmentName,
                                UnitId = tc.User.UnitId,
                                Unit_Name = tc.User.Units.UnitName,
                                ChannelId = tc.ChannelId,
                                Channel_Name = tc.Channel.ChannelName,
                                Concern_Description = tc.ConcernDetails,
                                CategoryId = tc.CategoryId,
                                Category_Description = tc.Category.CategoryDescription,
                                SubCategoryId = tc.SubCategoryId,
                                SubCategory_Description = tc.SubCategory.SubCategoryDescription,
                                tc.StartDate,
                                tc.TargetDate,
                                tc.Remarks,
                                tc.TicketType,
                                Ticket_Status = tc.IsApprove == true ? "Ticket Approve" : tc.IsReject ? "Rejected" : tc.ConcernStatus != TicketingConString.ForApprovalTicket ? tc.ConcernStatus : tc.IsApprove == false && tc.IsReject == false ? "For Approval" : "Unknown",
                                tc.IsAssigned
                            })
                            .Select(tc => new TicketRequestConcern
                            {
                                RequestTransactionId = tc.Key.RequestTransactionId,
                                DepartmentId = tc.Key.DepartmentId,
                                Department_Name = tc.Key.Department_Name,
                                UnitId = tc.Key.UnitId,
                                Unit_Name = tc.Key.Unit_Name,
                                ChannelId = tc.Key.ChannelId,
                                Channel_Name = tc.Key.Channel_Name,
                                Concern_Description = tc.Key.Concern_Description,
                                CategoryId = tc.Key.CategoryId,
                                Category_Description = tc.Key.Category_Description,
                                SubCategoryId = tc.Key.SubCategoryId,
                                SubCategory_Description = tc.Key.SubCategory_Description,
                                Start_Date = tc.Key.StartDate,
                                Target_Date = tc.Key.TargetDate,
                                Ticket_Status = tc.Key.Ticket_Status,
                                Remarks = tc.Key.Remarks,
                                Concern_Type = tc.Key.TicketType,
                                Is_Assigned = tc.Key.IsAssigned,
                                TicketConcerns = tc.Select(t => new TicketRequestConcern.TicketConcern
                                {
                                    TicketConcernId = t.Id,
                                    SubUnitId = t.User.SubUnitId,
                                    SubUnit_Name = t.User.SubUnit.SubUnitName,
                                    UserId = t.UserId,
                                    Issue_Handler = t.User.Fullname,
                                    Added_By = t.AddedByUser.Fullname,
                                    Created_At = t.CreatedAt,
                                    Modified_By = t.ModifiedByUser.Fullname,
                                    Updated_At = t.UpdatedAt,
                                    IsActive = t.IsActive,
                                }).ToList()
                            }).ToList()

                    });


               return await PagedList<GetRequestorTicketConcernResult>.CreateAsync(results, request.PageNumber, request.PageSize);


            }
        }
    }
}