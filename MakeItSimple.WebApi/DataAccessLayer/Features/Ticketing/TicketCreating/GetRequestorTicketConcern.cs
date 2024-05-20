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
                IQueryable<RequestConcern> requestConcernsQuery = _context.RequestConcerns.Include(x => x.User)
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
                     .ThenInclude(x => x.ChannelUsers);

                if (requestConcernsQuery.Count() > 0)
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
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.User.Fullname.Contains(request.Search)
                        && x.Id.ToString().Contains(request.Search));
                    }

                    if (request.Ascending != null)
                    {
                        if (request.Ascending is true)
                        {
                            requestConcernsQuery = requestConcernsQuery.OrderBy(x => x.RequestTransactionId);
                        }
                        else
                        {
                            requestConcernsQuery = requestConcernsQuery.OrderByDescending(x => x.RequestTransactionId);
                        }

                    }

                    if (request.Concern_Status != null)
                    {
                        if (request.Concern_Status == TicketingConString.Approval)
                        {
                            requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket);
                        }
                        else if (request.Concern_Status == TicketingConString.OnGoing)
                        {
                            requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing);
                        }
                        else if (request.Concern_Status == TicketingConString.Done)
                        {
                            requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.Done);
                        }
                        else
                        {
                            requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestTransactionId == null);
                        }

                    }

                    if(!string.IsNullOrEmpty(request.UserType))
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

                var results = requestConcernsQuery.Select(x => new GetRequestorTicketConcernResult
                {
                    RequestTransactionId = x.RequestTransactionId,
                    DepartmentId = x.User.DepartmentId,
                    Department_Name = x.User.Department.DepartmentName,
                    UserId = x.UserId,
                    EmpId = x.User.EmpId,
                    FullName = x.User.Fullname,
                    Concern = x.Concern,
                    RequestConcernId = x.Id,
                    Concern_Status = x.ConcernStatus,
                    Is_Done = x.IsDone,
                    Remarks = x.Remarks,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    updated_At = x.UpdatedAt,
                    TicketRequestConcerns = x.TicketConcerns.GroupBy(x => new
                    {
                        x.RequestTransactionId,
                        DepartmentId = x.User.DepartmentId,
                        Department_Name = x.User.Department.DepartmentName,
                        UnitId = x.User.UnitId,
                        Unit_Name = x.User.Units.UnitName,
                        ChannelId = x.ChannelId,
                        Channel_Name = x.Channel.ChannelName,
                        Concern_Description = x.ConcernDetails,
                        CategoryId = x.CategoryId,
                        Category_Description = x.Category.CategoryDescription,
                        SubCategoryId = x.SubCategoryId,
                        SubCategory_Description = x.SubCategory.SubCategoryDescription,
                        Start_Date = x.StartDate,
                        Target_Date = x.TargetDate,
                        Remarks = x.Remarks,
                        Concern_Type = x.TicketType,
                        Ticket_Status = x.IsApprove == true ? "Ticket Approve" : x.IsReject == true ? "Rejected" :
                         x.ConcernStatus != TicketingConString.ForApprovalTicket ? x.ConcernStatus : x.IsApprove == false
                         && x.IsReject == false ? "For Approval" : "Unknown",
                         x.IsAssigned


                    }).Select(x => new TicketRequestConcern
                    {
                        RequestTransactionId = x.Key.RequestTransactionId,
                        DepartmentId = x.Key.DepartmentId,
                        Department_Name = x.Key.Department_Name,
                        UnitId = x.Key.UnitId,
                        Unit_Name = x.Key.Unit_Name,
                        //SubUnitId = x.Key.SubUnitId,
                        //SubUnit_Name = x.Key.SubUnit_Name,
                        ChannelId = x.Key.ChannelId,
                        Channel_Name = x.Key.Channel_Name,
                        Concern_Description = x.Key.Concern_Description,
                        CategoryId = x.Key.CategoryId,
                        Category_Description = x.Key.Category_Description,
                        SubCategoryId = x.Key.SubCategoryId,
                        SubCategory_Description = x.Key.SubCategory_Description,
                        Start_Date = x.Key.Start_Date,
                        Target_Date = x.Key.Target_Date,
                        Ticket_Status = x.Key.Ticket_Status,
                        Remarks = x.Key.Remarks,
                        Concern_Type = x.Key.Concern_Type,
                        Is_Assigned = x.Key.IsAssigned,
                        TicketConcerns = x.Select(x => new TicketRequestConcern.TicketConcern
                        {
                            TicketConcernId = x.Id,
                            //ChannelUserId = x.Channel,
                            SubUnitId = x.User.SubUnitId,
                            SubUnit_Name = x.User.SubUnit.SubUnitName,
                            UserId = x.UserId,
                            Issue_Handler = x.User.Fullname,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Modified_By = x.ModifiedByUser.Fullname,
                            Updated_At = x.UpdatedAt,
                            IsActive = x.IsActive,

                        }).ToList(),


                    }).ToList(),

                });

                return await PagedList<GetRequestorTicketConcernResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}