using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.GetCompany.GetCompanyResult;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket.GetOpenTicket.GetOpenTicketResult.GetForClosingTicket;
namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket
{
    public partial class GetOpenTicket
    {

        public class Handler : IRequestHandler<GetOpenTicketQuery, PagedList<GetOpenTicketResult>>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOpenTicketResult>> Handle(GetOpenTicketQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
                    .AsNoTracking()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.User)
                    .ThenInclude(x => x.SubUnit)
                    .Include(x => x.ClosingTickets)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.TransferTicketConcerns)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.RequestConcern);


                if (ticketConcernQuery.Any())
                {

                    var fillterApproval = ticketConcernQuery
                        .Select(x => x.Id);

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

                    var issueHandlerPermissionList = allUserList
                        .Where(x => x.Permissions.Contains(TicketingConString.IssueHandler))
                        .Select(x => x.UserRoleName)
                        .ToList();

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        ticketConcernQuery = ticketConcernQuery
                            .Where(x => x.User.Fullname.Contains(request.Search)
                        || x.User.SubUnit.SubUnitName.Contains(request.Search));

                    }

                    if (request.Status is not null)
                    {
                        ticketConcernQuery = ticketConcernQuery
                            .Where(x => x.IsActive == request.Status);
                    }

                    if (!string.IsNullOrEmpty(request.Concern_Status))
                    {
                        switch (request.Concern_Status)
                        {
                            case TicketingConString.PendingRequest:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == false && x.OnHold != true);
                                break;

                            case TicketingConString.Open:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == true && x.IsTransfer != false
                                    && x.IsClosedApprove == null && x.OnHold != true);
                                break;

                            case TicketingConString.ForTransfer:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsTransfer == false && x.OnHold != true);
                                break;

                            case TicketingConString.OnHold:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.OnHold == true);
                                break;

                            case TicketingConString.ForClosing:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == false && x.OnHold != true);
                                break;

                            case TicketingConString.NotConfirm:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold != true);
                                break;

                            case TicketingConString.Closed:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold != true);
                                break;

                            default:
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);

                        }
                    }

                    if (!string.IsNullOrEmpty(request.History_Status))
                    {
                        switch (request.History_Status)
                        {
                            case TicketingConString.PendingRequest:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == false && x.OnHold != true);
                                break;

                            case TicketingConString.Open:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == true && x.IsTransfer != false
                                    && x.IsClosedApprove == null && x.OnHold != true);
                                break;

                            case TicketingConString.ForTransfer:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsTransfer == false && x.OnHold != true);
                                break;

                            case TicketingConString.OnHold:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.OnHold == true);
                                break;

                            case TicketingConString.ForClosing:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == false && x.OnHold != true);
                                break;

                            case TicketingConString.NotConfirm:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold != true);
                                break;

                            case TicketingConString.Closed:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold != true);
                                break;

                            default:
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);

                        }
                    }

                    if (request.Date_From is not null && request.Date_To is not null)
                    {
                        ticketConcernQuery = ticketConcernQuery
                            .Where(x => x.TargetDate >= request.Date_From.Value && x.TargetDate <= request.Date_To.Value);
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {

                        if (request.UserType == TicketingConString.IssueHandler)
                        {
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.UserId == request.UserId);
                        }
                        else if (request.UserType == TicketingConString.Receiver)
                        {
                            var listOfRequest = await ticketConcernQuery
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
                                .Where(x => businessUnitDefault.Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
                                 x.UserId == request.UserId)
                                .Select(x => x.BusinessUnitId)
                                .ToListAsync();

                            if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList.Any())
                            {

                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => receiverList.Contains(x.RequestorByUser.BusinessUnitId));
                            }
                            else
                            {
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                            }

                        }
                        else
                        {
                            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                        }
                    }

                }

                var results = ticketConcernQuery
                    .Select(x => new GetOpenTicketResult
                    {

                        Closed_Status = x.TargetDate.Value.Day >= x.Closed_At.Value.Day && x.IsClosedApprove == true
                        ? TicketingConString.OnTime : x.TargetDate.Value.Day < x.Closed_At.Value.Day && x.IsClosedApprove == true
                        ? TicketingConString.Delay : null,
                        TicketConcernId = x.Id,
                        RequestConcernId = x.RequestConcernId,
                        Concern_Description = x.RequestConcern.Concern,
                        Company_Code = x.RequestConcern.Company.CompanyCode,
                        Company_Name = x.RequestConcern.Company.CompanyName,
                        BusinessUnit_Code = x.RequestConcern.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = x.RequestConcern.BusinessUnit.BusinessName,
                        Department_Code = x.RequestorByUser.Department.DepartmentCode,
                        Department_Name = x.RequestorByUser.Department.DepartmentName,
                        Unit_Code = x.RequestConcern.Unit.UnitCode,
                        Unit_Name = x.RequestConcern.Unit.UnitName,
                        SubUnit_Code = x.RequestorByUser.SubUnit.SubUnitCode,
                        SubUnit_Name = x.User.SubUnit.SubUnitName,
                        Location_Code = x.RequestConcern.Location.LocationCode,
                        Location_Name = x.RequestConcern.Location.LocationName,
                        Requestor_By = x.RequestorBy,
                        Requestor_Name = x.RequestorByUser.Fullname,
                        Category_Description = x.RequestConcern.Category.CategoryDescription,
                        SubCategory_Description = x.RequestConcern.SubCategory.SubCategoryDescription,
                        Date_Needed = x.RequestConcern.DateNeeded,
                        Notes = x.RequestConcern.Notes,
                        Contact_Number = x.RequestConcern.ContactNumber,
                        Request_Type = x.RequestConcern.RequestType,
                        Channel_Name = x.RequestConcern.Channel.ChannelName,
                        UserId = x.UserId,
                        Issue_Handler = x.User.Fullname,
                        Target_Date = x.TargetDate,
                        Ticket_Status = x.IsApprove == false && x.OnHold != true ? TicketingConString.PendingRequest
                                        : x.IsApprove == true != false && x.IsTransfer != false && x.IsClosedApprove == null && x.OnHold != true ? TicketingConString.OpenTicket
                                        : x.IsTransfer == false && x.OnHold != true ? TicketingConString.ForTransfer
                                        : x.OnHold == true && x.OnHold != true ? TicketingConString.OnHold
                                        : x.IsClosedApprove == false && x.OnHold != true ? TicketingConString.ForClosing
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold != true ? TicketingConString.NotConfirm
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold != true ? TicketingConString.Closed
                                        : "Unknown",

                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.CreatedAt,
                        Remarks = x.Remarks,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt,
                        IsActive = x.IsActive,
                        Is_Closed = x.IsClosedApprove,
                        Closed_At = x.Closed_At,
                        Is_Transfer = x.IsTransfer,
                        Transfer_At = x.TransferAt,
                        GetForClosingTickets = x.ClosingTickets
                       .Where(x => x.IsActive == true && x.IsRejectClosed == false)
                       .Where(x => x.IsClosing == true ? x.IsClosing == true : x.IsClosing == false)
                      .Select(x => new GetOpenTicketResult.GetForClosingTicket
                      {
                          ClosingTicketId = x.Id,
                          Remarks = x.RejectRemarks,
                          Resolution = x.Resolution,
                          CategoryId = x.CategoryId,
                          Category_Description = x.Category.CategoryDescription,
                          SubCategoryId = x.SubCategoryId,
                          SubCategory_Description = x.SubCategory.SubCategoryDescription,
                          Notes = x.Notes,
                          IsApprove = x.ApproverTickets.Any(x => x.IsApprove == null) ? false : true,
                          ApproverLists = x.ApproverTickets
                          .Where(x => x.IsApprove != null)
                          .Select(x => new ApproverList
                          {
                              ApproverName = x.User.Fullname,
                              Approver_Level = x.ApproverLevel.Value

                          }).OrderByDescending(x => x.Approver_Level)
                          .ToList(),

                          GetAttachmentForClosingTickets = x.TicketAttachments.Select(x => new GetAttachmentForClosingTicket
                          {
                              TicketAttachmentId = x.Id,
                              Attachment = x.Attachment,
                              FileName = x.FileName,
                              FileSize = x.FileSize,

                          }).ToList(),


                      })
                      .ToList(),

                        GetOnHolds = x.TicketOnHolds
                        .Where(x => x.IsHold == true)

                        .Select(h => new GetOpenTicketResult.GetOnHold
                        {
                            Id = h.Id,
                            Reason = h.Reason,
                            AddedBy = h.AddedByUser.Fullname,
                            CreatedAt = h.CreatedAt,
                            IsHold = h.IsHold,
                            ResumeAt = h.ResumeAt,
                            GetAttachmentForOnHoldTickets = h.TicketAttachments
                            .Select(t => new GetOpenTicketResult.GetOnHold.GetAttachmentForOnHoldTicket
                            {
                                TicketAttachmentId = t.Id,
                                Attachment = t.Attachment,
                                FileName = t.FileName,
                                FileSize = t.FileSize,

                            }).ToList(),

                        }).ToList(),

                        GetForTransferTickets = x.TransferTicketConcerns
                        .Where(x => x.IsActive == true && x.IsTransfer == false)
                        .Select(x => new GetOpenTicketResult.GetForTransferTicket
                        {
                            TransferTicketConcernId = x.Id,
                            Transfer_Remarks = x.TransferRemarks,
                            IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
                            GetAttachmentForTransferTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForTransferTicket.GetAttachmentForTransferTicket
                            {
                                TicketAttachmentId = x.Id,
                                Attachment = x.Attachment,
                                FileName = x.FileName,
                                FileSize = x.FileSize,

                            }).ToList(),

                        })
                        .ToList(),

                        Transaction_Date = x.ticketHistories.Max(x => x.TransactionDate).Value,

                    }).OrderBy(x => x.Transaction_Date); 


                return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
