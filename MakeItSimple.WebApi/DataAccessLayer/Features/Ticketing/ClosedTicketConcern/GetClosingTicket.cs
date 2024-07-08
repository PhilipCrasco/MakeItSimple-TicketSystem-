using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class GetClosingTicket
    {

        public class GetClosingTicketResults
        {
            //public int? TicketTransactionId { get; set; }
            public int ClosingTicketId { get; set; }
            public int TicketConcernId { get; set; }
            public string Resolution {  get; set; }
            public int ? DepartmentId { get; set; }
            public string Department_Name { get; set; }
            public int ? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }
            public string Concern_Details { get; set; }
            public string Category_Description { get; set; }
            public string SubCategoryDescription { get; set; }
            public int? Delay_Days { get; set; }
            public string Closed_By { get; set; }
            public DateTime? Closed_At { get; set; }
            public string Closed_Status { get; set; }
            public string Closed_Remarks { get; set; }
            public string RejectClosed_By { get; set; }
            public DateTime? RejectClosed_At { get; set; }
            public string Reject_Remarks { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? Start_Date { get; set; }
            public DateTime? Target_Date { get; set; }

            public List<ClosingAttachment> ClosingAttachments { get; set; }

            public class ClosingAttachment
            {
                public int ? TicketAttachmentId { get; set; }
                public string Attachment { get; set; }
                public string FileName { get; set; }
                public decimal? FileSize { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }
            }
            
        }


        public class GetClosingTicketQuery : UserParams , IRequest<PagedList<GetClosingTicketResults>>
        {
            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public string Search { get; set; }
            public bool? IsClosed { get; set; }
            public bool? IsReject { get; set; }

        }

        public class Handler : IRequestHandler<GetClosingTicketQuery, PagedList<GetClosingTicketResults>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetClosingTicketResults>> Handle(GetClosingTicketQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                IQueryable<ClosingTicket> closingTicketsQuery = _context.ClosingTickets
                    .Include(x => x.AddedByUser)
                    .Include(x => x.RejectClosedByUser)
                    .Include(x => x.ClosedByUser)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User) 
                    .ThenInclude(x => x.Department)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .ThenInclude(x => x.SubUnit)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.Channel)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.Category)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.SubCategory);



                if (closingTicketsQuery.Count() > 0)
                {

                    var allUserList = await _context.UserRoles.ToListAsync();

                    var receiverPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                    var approverPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        closingTicketsQuery = closingTicketsQuery
                            .Where(x => x.TicketConcern.User.Fullname.Contains(request.Search)
                                        || x.TicketConcernId.ToString().Contains(request.Search));
                    }

                    if (request.IsReject != null)
                    {
                        closingTicketsQuery = closingTicketsQuery.Where(x => x.IsRejectClosed == request.IsReject);
                    }

                    if (request.IsClosed != null)
                    {
                        closingTicketsQuery = closingTicketsQuery.Where(x => x.IsClosing == request.IsClosed);
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {

                        var closingTicket = closingTicketsQuery
                            .Select(x => x.TicketConcern.User.BusinessUnitId);

                        var receiverList = await _context.Receivers
                            .Include(x => x.User)
                            .FirstOrDefaultAsync(x => closingTicket.Contains(x.BusinessUnitId));

                        if (request.UserType == TicketingConString.Approver)
                        {

                            if (approverPermissionList.Any(x => x.Contains(request.Role)))
                            {

                                var userApprover = await _context.Users
                                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                                var approverTransactList = await _context.ApproverTicketings
                                    .Where(x => x.UserId == userApprover.Id)
                                    .ToListAsync();

                                var approvalLevelList = approverTransactList
                                    .Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null)
                                    .ToList();

                                var userRequestIdApprovalList = approvalLevelList.Select(x => x.ClosingTicketId);
                                var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);

                                closingTicketsQuery = closingTicketsQuery
                                    .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                                    && userRequestIdApprovalList.Contains(x.Id));

                            }
                        }

                        else if(request.UserType == TicketingConString.Receiver)
                        {
                            if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList != null)
                            {
                                if (request.UserId == receiverList.UserId)
                                {  
                                    var filterApproval = closingTicketsQuery.Select(x => x.Id);
                                     
                                    var approverTransactList = await _context.ApproverTicketings
                                        .Where(x => filterApproval.Contains(x.ClosingTicketId.Value) && x.IsApprove == null)
                                        .ToListAsync();

                                    if (approverTransactList.Any())
                                    {
                                        var generatedIdInApprovalList = approverTransactList
                                            .Select(approval => approval.ClosingTicketId);

                                        closingTicketsQuery = closingTicketsQuery
                                            .Where(x => !generatedIdInApprovalList.Contains(x.Id));

                                        
                                    }

                                    var receiver = await _context.TicketConcerns
                                        .Include(x => x.RequestorByUser)
                                        .Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId)
                                        .ToListAsync();

                                    var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);

                                    closingTicketsQuery = closingTicketsQuery
                                        .Where(x => receiverContains.Contains(x.TicketConcern.User.BusinessUnitId));
                                }
                                else
                                {
                                    return new PagedList<GetClosingTicketResults>(new List<GetClosingTicketResults>(), 0, request.PageNumber, request.PageSize);
                                }

                            }

                        }
                        else if (request.UserType == TicketingConString.Users)
                        {
                            closingTicketsQuery = closingTicketsQuery.Where(x => x.AddedByUser.Id == request.UserId);
                        }
                        else
                        {
                            return new PagedList<GetClosingTicketResults>(new List<GetClosingTicketResults>(), 0, request.PageNumber, request.PageSize);
                        }

                    }

                }

                var results = closingTicketsQuery
                    .Select(x => new GetClosingTicketResults
                    {
                        ClosingTicketId = x.Id,
                        TicketConcernId = x.TicketConcernId,
                        Resolution = x.Resolution,
                        DepartmentId = x.TicketConcern.User.DepartmentId,
                        Department_Name = x.TicketConcern.User.Department.DepartmentName,
                        ChannelId = x.TicketConcern.ChannelId,
                        Channel_Name = x.TicketConcern.Channel.ChannelName,
                        UserId = x.TicketConcern.UserId,   
                        Fullname = x.TicketConcern.User.Fullname,
                        Concern_Details = x.TicketConcern.ConcernDetails,
                        Category_Description = x.TicketConcern.Category.CategoryDescription,
                        SubCategoryDescription = x.TicketConcern.SubCategory.SubCategoryDescription,
                        RejectClosed_By = x.RejectClosedByUser.Fullname,
                        RejectClosed_At= x.RejectClosedAt,
                        Reject_Remarks = x.RejectRemarks,
                        Closed_By = x.ClosedByUser.Fullname,
                        Closed_At = x.ClosingAt,
                        Closed_Remarks = x.ClosingRemarks,
                        Start_Date = x.TicketConcern.StartDate,
                        Target_Date = x.TicketConcern.TargetDate,
                        Delay_Days = x.TicketConcern.TargetDate < dateToday && x.ClosingAt == null ? Microsoft.EntityFrameworkCore.SqlServerDbFunctionsExtensions.DateDiffDay(EF.Functions, x.TicketConcern.TargetDate, dateToday)
                            : x.TicketConcern.TargetDate < x.ClosingAt && x.ClosingAt != null ? Microsoft.EntityFrameworkCore.SqlServerDbFunctionsExtensions.DateDiffDay(EF.Functions, x.TicketConcern.TargetDate, x.ClosingAt) : 0,

                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.CreatedAt,
                        Updated_At = x.UpdatedAt,
                        Modified_By = x.ModifiedByUser.Fullname,

                        ClosingAttachments = x.TicketAttachments.Select(x => new GetClosingTicketResults.ClosingAttachment
                        {

                            TicketAttachmentId = x.Id,
                            Attachment = x.Attachment,
                            FileName = x.FileName,
                            FileSize = x.FileSize,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Modified_By = x.ModifiedByUser.Fullname,
                            Updated_At = x.UpdatedAt,

                        }).ToList(),


                    });

                return await PagedList<GetClosingTicketResults>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }



}
