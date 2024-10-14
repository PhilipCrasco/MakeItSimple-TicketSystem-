using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.GetCompany.GetCompanyResult;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket.GetRequestorTicketConcern.GetRequestorTicketConcernResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket
{
    public partial class GetRequestorTicketConcern
    {

        public class Handler : IRequestHandler<GetRequestorTicketConcernQuery, PagedList<GetRequestorTicketConcernResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetRequestorTicketConcernResult>> Handle(GetRequestorTicketConcernQuery request, CancellationToken cancellationToken)
            {

                var dateToday = DateTime.Now;

                IQueryable<RequestConcern> requestConcernsQuery = _context.RequestConcerns
                     .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.User)
                     .Include(x => x.AddedByUser)
                     .Include(x => x.ModifiedByUser)
                     .Include(x => x.Company)
                     .Include(x => x.BusinessUnit)
                     .Include(x => x.Department)
                     .Include(x => x.Unit)
                     .Include(x => x.SubUnit)
                     .Include(x => x.Location)
                     .Include(x => x.Category)
                     .Include(x => x.SubCategory)
                     .Include(x => x.TicketConcerns)
                     .ThenInclude(x => x.Channel)
                     .Include(x => x.TicketConcerns)
                     .ThenInclude(x => x.User)
                     .OrderBy(x => x.Id)
                     .AsSplitQuery()
                     ;


                if (requestConcernsQuery.Any())
                {

                    var allUserList = await _context.UserRoles
                        .AsNoTrackingWithIdentityResolution()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions

                        }).ToListAsync();


                    var receiverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                    var issueHandlerPermissionList = allUserList
                   .Where(x => x.Permissions
                   .Contains(TicketingConString.IssueHandler))
                   .Select(x => x.UserRoleName)
                   .ToList();

                    var requestorPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor))
                    .Select(x => x.UserRoleName)
                    .ToList();


                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.User.Fullname
                            .Contains(request.Search)
                            || x.Id.ToString().Contains(request.Search)
                            || x.Concern.Contains(request.Search));
                    }

                    if (request.Status != null)
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.IsActive == request.Status);
                    }

                    if (request.Is_Approve != null)
                    {
                        var ticketStatusList = await _context.TicketConcerns
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x => x.IsApprove == request.Is_Approve)
                            .Select(x => x.RequestConcernId)
                            .ToListAsync();

                        requestConcernsQuery = requestConcernsQuery
                           .Where(x => ticketStatusList.Contains(x.Id));
                    }


                    if (request.Ascending != null)
                    {
                        requestConcernsQuery = request.Ascending.Value
                            ? requestConcernsQuery
                            .OrderBy(x => x.Id)
                            : requestConcernsQuery
                            .OrderByDescending(x => x.Id);
                    }

                    if (request.Concern_Status is not null)
                    {

                        switch (request.Concern_Status)
                        {

                            case TicketingConString.Approval:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket);
                                break;
                            case TicketingConString.OnGoing:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing);
                                break;

                            case TicketingConString.NotConfirm:
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm);
                                break;

                            case TicketingConString.Done:
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.ConcernStatus == TicketingConString.Done && x.Is_Confirm == true);
                                break;
                            default:
                                return new PagedList<GetRequestorTicketConcernResult>(new List<GetRequestorTicketConcernResult>(), 0, request.PageNumber, request.PageSize);

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
                                return new PagedList<GetRequestorTicketConcernResult>(new List<GetRequestorTicketConcernResult>(), 0, request.PageNumber, request.PageSize);
                            }
                        }

                        if (request.UserType == TicketingConString.Receiver && requestConcernsQuery.Any())
                        {
                            var listOfRequest = await requestConcernsQuery
                                .Select(x => x.User.BusinessUnitId)
                                .ToListAsync();

                            var businessUnitDefault = await _context.BusinessUnits
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x => x.IsActive == true)
                            .Where(x => listOfRequest.Contains(x.Id))
                            .Select(x => x.Id)
                            .ToListAsync();

                            var receiverList = await _context.Receivers
                                .AsNoTrackingWithIdentityResolution()
                                .Include(x => x.BusinessUnit)
                                .Where(x => businessUnitDefault
                                .Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
                                 x.UserId == request.UserId)
                                .Select(x => x.BusinessUnitId)
                                .ToListAsync();

                            if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList.Any())
                            {

                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => receiverList.Contains(x.User.BusinessUnitId));

                            }
                            else
                            {
                                return new PagedList<GetRequestorTicketConcernResult>(new List<GetRequestorTicketConcernResult>(), 0, request.PageNumber, request.PageSize);
                            }
                        }

                    }


                }

                var results = requestConcernsQuery
                    .Select(g => new GetRequestorTicketConcernResult
                    {

                        RequestConcernId = g.Id,
                        Concern = g.Concern,
                        Resolution = g.Resolution,

                        CompanyId = g.CompanyId,
                        Company_Code = g.Company.CompanyCode,
                        Company_Name = g.Company.CompanyName,

                        BusineesUnitId = g.BusinessUnitId,
                        BusinessUnit_Code = g.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = g.BusinessUnit.BusinessName,

                        DepartmentId = g.DepartmentId,
                        Department_Code = g.Department.DepartmentCode,
                        Department_Name = g.Department.DepartmentName,

                        UnitId = g.UnitId,
                        Unit_Code = g.Unit.UnitCode,
                        Unit_Name = g.Unit.UnitName,

                        SubUnitId = g.SubUnitId,
                        SubUnit_Code = g.SubUnit.SubUnitCode,
                        SubUnit_Name = g.SubUnit.SubUnitName,

                        LocationId = g.LocationId,
                        Location_Code = g.Location.LocationCode,
                        Location_Name = g.Location.LocationName,

                        RequestorId = g.UserId,
                        FullName = g.User.Fullname,

                        CategoryId = g.CategoryId,
                        Category_Description = g.Category.CategoryDescription,

                        SubCategoryId = g.SubCategoryId,
                        SubCategory_Description = g.SubCategory.SubCategoryDescription,

                        Concern_Status = g.ConcernStatus,
                        Is_Done = g.IsDone,
                        Remarks = g.Remarks,
                        Notes = g.Notes,
                        Contact_Number = g.ContactNumber,
                        Request_Type = g.RequestType,
                        Added_By = g.AddedByUser.Fullname,
                        Date_Needed = g.DateNeeded,
                        Created_At = g.CreatedAt,
                        Modified_By = g.ModifiedByUser.Fullname,
                        updated_At = g.UpdatedAt,
                        Is_Confirmed = g.Is_Confirm,
                        Confirmed_At = g.Confirm_At,
                        TicketRequestConcerns = g.TicketConcerns
                            .Select(tc => new TicketRequestConcern
                            {

                                TicketConcernId = tc.Id,
                                ChannelId = tc.ChannelId,
                                Channel_Name = tc.Channel.ChannelName,
                                UserId = tc.UserId,
                                Issue_Handler = tc.User.Fullname,
                                Target_Date = tc.TargetDate,
                                Remarks = tc.Remarks,
                                Is_Assigned = tc.IsAssigned,
                                Added_By = tc.AddedByUser.Fullname,
                                Created_At = tc.CreatedAt,
                                Modified_By = tc.ModifiedByUser.Fullname,
                                Updated_At = tc.UpdatedAt,
                                Is_Active = tc.IsActive,
                                Closed_At = tc.Closed_At,
                                Closing_Notes = tc.IsClosedApprove == true ? 
                                tc.ClosingTickets.Where(x => x.IsClosing == true)
                                .OrderByDescending(x => x.ClosingAt)
                                .First().Notes : null,
                                Is_Transfer = tc.IsTransfer,
                                Transfer_At = tc.TransferAt,
                                Transfer_By = tc.TransferByUser.Fullname,

                            }).ToList()

                    });



                return await PagedList<GetRequestorTicketConcernResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}