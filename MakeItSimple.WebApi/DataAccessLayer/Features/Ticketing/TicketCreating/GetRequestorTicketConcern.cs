﻿using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.GetCompany.GetCompanyResult;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetRequestorTicketConcern.GetRequestorTicketConcernResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class GetRequestorTicketConcern
    {
        public class GetRequestorTicketConcernResult
        {

            //public int? RequestConcernCount { get; set; }
            //public int? ForApprovalCount { get; set; }
            //public int? OnGoingCount { get; set; }
            //public int? ForConfirmationCount { get; set; }
            //public int? DoneCount { get; set; }

            public int? RequestConcernId { get; set; }
            public string Concern { get; set; }
            public string Resolution { get; set; }
            public int? DepartmentId { get; set; }
            public string Department_Name { get; set; }
            public Guid? RequestorId { get; set; }
            public string EmpId { get; set; }
            public string FullName { get; set; }
            public string Concern_Status { get; set; }
            public bool? Is_Done { get; set; }
            public string Remarks { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? updated_At { get; set; }
            public bool? Is_Confirmed { get; set; }
            public DateTime? Confirmed_At { get; set; }

            public List<TicketRequestConcern> TicketRequestConcerns { get; set; }
            public class TicketRequestConcern
            {

                public int? TicketConcernId { get; set; }
                public string Ticket_No { get; set; }
                public string Concern_Description { get; set; }
                public int? DepartmentId { get; set; }
                public string Department_Name { get; set; }
                public int? ChannelId { get; set; }
                public string Channel_Name { get; set; }
                public int? UnitId { get; set; }
                public string Unit_Name { get; set; }
                public int? SubUnitId { get; set; }
                public string SubUnit_Name { get; set; }
                public Guid? UserId { get; set; }
                public string Issue_Handler { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
                public DateTime? Start_Date { get; set; }
                public DateTime? Target_Date { get; set; }
                public string Ticket_Status { get; set; }
                public bool? Is_Assigned { get; set; }
                //public string Concern_Status {  get; set; }
                public string Remarks { get; set; }
                public string Concern_Type { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }
                public bool Is_Active { get; set; }
                public bool Is_Reject { get; set; }
                public DateTime? Closed_At { get; set; }
                public bool? Is_Transfer { get; set; }
                public DateTime? Transfer_At { get; set; }
                public string Transfer_By { get; set; }

            }

        }


        public class GetRequestorTicketConcernQuery : UserParams, IRequest<PagedList<GetRequestorTicketConcernResult>>
        {

            public string UserType { get; set; }
            public string Role { get; set; }
            public Guid? UserId { get; set; }
            public string Concern_Status { get; set; }
            public string Search { get; set; }
            public bool? Status { get; set; }
            public bool? Is_Reject { get; set; }
            public bool? Is_Approve { get; set; }
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
                int hourDif = 24;
                var dateToday = DateTime.Now;

                IQueryable<RequestConcern> requestConcernsQuery = _context.RequestConcerns
                    .AsNoTracking()
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
                     .Include(x => x.TicketConcerns)
                     .ThenInclude(x => x.TransferByUser);

                if (requestConcernsQuery.Any())
                {

                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
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


                    if (request.Is_Reject != null)
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.IsReject == request.Is_Reject);
                    }

                    if (request.Ascending != null)
                    {
                        requestConcernsQuery = request.Ascending.Value
                            ? requestConcernsQuery
                            .OrderBy(x => x.Id)
                            : requestConcernsQuery
                            .OrderByDescending(x => x.Id);
                    }

                    if (request.Concern_Status != null)
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
                        DepartmentId = g.User.DepartmentId,
                        Department_Name = g.User.Department.DepartmentName,
                        RequestorId = g.UserId,
                        EmpId = g.User.EmpId,
                        FullName = g.User.Fullname,
                        Concern_Status = g.ConcernStatus,
                        Is_Done = g.IsDone,
                        Remarks = g.Remarks,
                        Added_By = g.AddedByUser.Fullname,
                        Created_At = g.CreatedAt,
                        Modified_By = g.ModifiedByUser.Fullname,
                        updated_At = g.UpdatedAt,
                        Is_Confirmed = g.Is_Confirm,
                        Confirmed_At = g.Confirm_At,
                        TicketRequestConcerns = g.TicketConcerns
                            .Select(tc => new TicketRequestConcern
                            {

                                TicketConcernId = tc.Id,
                                Ticket_No = tc.TicketNo,
                                Concern_Description = tc.ConcernDetails,
                                DepartmentId = tc.User.DepartmentId,
                                Department_Name = tc.User.Department.DepartmentName,
                                UnitId = tc.User.UnitId,
                                Unit_Name = tc.User.Units.UnitName,
                                SubUnitId = tc.User.SubUnitId,
                                SubUnit_Name = tc.User.SubUnit.SubUnitName,
                                ChannelId = tc.ChannelId,
                                Channel_Name = tc.Channel.ChannelName,
                                UserId = tc.UserId,
                                Issue_Handler = tc.User.Fullname,
                                CategoryId = tc.CategoryId,
                                Category_Description = tc.Category.CategoryDescription,
                                SubCategoryId = tc.SubCategoryId,
                                SubCategory_Description = tc.SubCategory.SubCategoryDescription,
                                Start_Date = tc.StartDate,
                                Target_Date = tc.TargetDate,
                                Remarks = tc.Remarks,
                                Concern_Type = tc.TicketType,
                                Ticket_Status = tc.IsApprove == true ? "Ticket Approve" : tc.IsReject ? "Rejected" :
                                tc.ConcernStatus != TicketingConString.ForApprovalTicket ? tc.ConcernStatus
                                : tc.IsApprove == false && tc.IsReject == false ? "For Approval" : "Unknown",

                                Is_Assigned = tc.IsAssigned,
                                Added_By = tc.AddedByUser.Fullname,
                                Created_At = tc.CreatedAt,
                                Modified_By = tc.ModifiedByUser.Fullname,
                                Updated_At = tc.UpdatedAt,
                                Is_Active = tc.IsActive,
                                Is_Reject = tc.IsReject,
                                Closed_At = tc.Closed_At,
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