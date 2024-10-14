using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetClosing
{
    public partial class GetClosingTicket
    {

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
                var businessUnitList = new List<BusinessUnit>();

                IQueryable<ClosingTicket> closingTicketsQuery = _context.ClosingTickets
                    .AsNoTracking()
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
                    .ThenInclude(x => x.RequestConcern)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.RequestorByUser);

                if (closingTicketsQuery.Any())
                {

                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions

                        })
                        .ToListAsync();

                    var receiverPermissionList = allUserList
                        .Where(x => x.Permissions
                        .Contains(TicketingConString.Receiver))
                        .Select(x => x.UserRoleName)
                        .ToList();

                    var approverPermissionList = allUserList
                        .Where(x => x.Permissions
                        .Contains(TicketingConString.Approver))
                        .Select(x => x.UserRoleName)
                        .ToList();

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
                        var filterApproval = closingTicketsQuery.Select(x => x.Id);

                        if (request.UserType == TicketingConString.Approver)
                        {

                            if (approverPermissionList.Any(x => x.Contains(request.Role)))
                            {

                                var userApprover = await _context.Users
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                                var approverTransactList = await _context.ApproverTicketings
                                    .AsNoTracking()
                                    .Where(x => x.UserId == userApprover.Id)
                                    .Where(x => x.IsApprove == null)
                                    .Select(x => new
                                    {
                                        x.ApproverLevel,
                                        x.IsApprove,
                                        x.ClosingTicketId,
                                        x.UserId,

                                    })
                                    .ToListAsync();

                                var userRequestIdApprovalList = approverTransactList.Select(x => x.ClosingTicketId);
                                var userIdsInApprovalList = approverTransactList.Select(approval => approval.UserId);

                                closingTicketsQuery = closingTicketsQuery
                                    .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                                    && userRequestIdApprovalList.Contains(x.Id));

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
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new GetClosingTicketResults
                    {
                        ClosingTicketId = x.Id,
                        TicketConcernId = x.TicketConcernId,
                        Resolution = x.Resolution,
                        Notes = x.Notes,
                        DepartmentId = x.TicketConcern.User.DepartmentId,
                        Department_Name = x.TicketConcern.User.Department.DepartmentName,
                        ChannelId = x.TicketConcern.ChannelId,
                        Channel_Name = x.TicketConcern.Channel.ChannelName,
                        UserId = x.TicketConcern.UserId,
                        Fullname = x.TicketConcern.User.Fullname,
                        Concern_Details = x.TicketConcern.RequestConcern.Concern,
                        Category_Description = x.TicketConcern.RequestConcern.Category.CategoryDescription,
                        SubCategoryDescription = x.TicketConcern.RequestConcern.SubCategory.SubCategoryDescription,
                        RejectClosed_By = x.RejectClosedByUser.Fullname,
                        RejectClosed_At = x.RejectClosedAt,
                        Reject_Remarks = x.RejectRemarks,
                        Closed_By = x.ClosedByUser.Fullname,
                        Closed_At = x.ClosingAt,
                        Closed_Remarks = x.ClosingRemarks,
                        Target_Date = x.TicketConcern.TargetDate,
                        Delay_Days = x.TicketConcern.TargetDate < dateToday && x.ClosingAt == null ? EF.Functions.DateDiffDay(x.TicketConcern.TargetDate, dateToday)
                            : x.TicketConcern.TargetDate < x.ClosingAt && x.ClosingAt != null ? EF.Functions.DateDiffDay(x.TicketConcern.TargetDate, x.ClosingAt) : 0,

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

                        }).ToList()

                    });

                return await PagedList<GetClosingTicketResults>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }

}
