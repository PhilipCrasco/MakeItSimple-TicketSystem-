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
            public int? RequestGeneratorId { get; set; }
            public int ? DepartmentId { get; set; }
            public string Department_Name { get; set; }
            public int ? UnitId { get; set; }
            public string Unit_Name { get; set; }
            public int ? SubUnitId { get; set; }
            public string SubUnit_Name { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid ? Requestor_By { get; set; }
            public string Requestor_Name { get; set; }
            public string Ticket_Status { get; set; }
            public string Remarks { get; set; }
            public string Concern_Type { get; set; }
            public bool ? Done { get; set; }
            public List<GetRequestConcernByConcern> GetRequestConcernByConcerns { get; set; }


            public class GetRequestConcernByConcern
            {

                public int? TicketConcernId { get; set; }
                public Guid? UserId { get; set; }
                public string Issue_Handler { get; set; }
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
                public string IssueHandler { get; set; }
                public bool ? Approval { get; set; }
                public string Search { get; set; }
                public bool ? Status { get; set; }
                public bool ? Reject { get; set; }
                public string Approver { get; set; }
                public string Requestor { get; set; }

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
                        .Include(x => x.User)
                        .Include(x => x.RequestorByUser);
;


                    var businessUnitList = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == ticketConcernQuery.First().RequestorByUser.BusinessUnitId);
                    var receiverList = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                    var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                    var fillterApproval = ticketConcernQuery.Select(x => x.RequestGeneratorId);


                    if (request.Search != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestorByUser.Fullname.Contains(request.Search));
                    }

                    if (request.Status != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == request.Status);
                    }

                    if(request.Reject != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == request.Reject);
                    }

                    if (request.Approval != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsApprove == request.Approval);
                    }



                    if (request.Approver == TicketingConString.Approver)
                    {
                       if(request.Role == TicketingConString.Receiver && receiverList != null)
                        {
                            if(request.UserId == receiverList.UserId)
                            {
                                var approverTransactList = await _context.ApproverTicketings.Where(x => fillterApproval.Contains(x.RequestGeneratorId) && x.IsApprove == null).ToListAsync();

                                if (approverTransactList != null && approverTransactList.Any())
                                {
                                    var generatedIdInApprovalList = approverTransactList.Select(approval => approval.RequestGeneratorId);
                                    ticketConcernQuery = ticketConcernQuery.Where(x => !generatedIdInApprovalList.Contains(x.RequestGeneratorId));
                                }
                                var receiver = await _context.TicketConcerns.Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId).ToListAsync();
                                var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);
                                ticketConcernQuery = ticketConcernQuery.Where(x => receiverContains.Contains(x.RequestorByUser.BusinessUnitId));
                            }
                            else
                            {
                                ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestGeneratorId == null);
                            }

                       }

                        else if (request.Role == TicketingConString.Approver)
                        {
                            var approverTransactList = await _context.ApproverTicketings.Where(x => x.UserId == userApprover.Id).ToListAsync();
                            var approvalLevelList = approverTransactList.Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null).ToList();
                            var userRequestIdApprovalList = approvalLevelList.Select(x => x.RequestGeneratorId);
                            var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                            ticketConcernQuery = ticketConcernQuery.Where(x => userIdsInApprovalList.Contains(x.TicketApprover) && userRequestIdApprovalList.Contains(x.RequestGeneratorId));

                        }
                        else
                        {
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestGeneratorId == null);
                        }

                    }


                    if(request.Requestor == TicketingConString.Requestor)
                    {

                        var requestConcernList = await _context.RequestConcerns.Where(x => x.UserId == request.UserId).ToListAsync();
                        var requestConcernContains = requestConcernList.Select(x => x.UserId);
                        ticketConcernQuery = ticketConcernQuery.Where(x => requestConcernContains.Contains(x.RequestorBy));

                    }

                    if(request.IssueHandler == TicketingConString.IssueHandler)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.UserId == request.UserId);
                    }


                    var result = ticketConcernQuery.GroupBy(x => x.RequestGeneratorId).Select(x => new GetRequestConcernResult
                    {

                        RequestGeneratorId = x.Key,
                        DepartmentId = x.First().RequestorByUser.DepartmentId,
                        Department_Name = x.First().RequestorByUser.Department.DepartmentName,
                        UnitId = x.First().User.UnitId,
                        Unit_Name = x.First().User.Units.UnitName,
                        SubUnitId = x.First().User.SubUnitId,
                        SubUnit_Name = x.First().User.SubUnit.SubUnitName,
                        ChannelId = x.First().ChannelId,
                        Channel_Name = x.First().Channel.ChannelName,
                        Requestor_By = x.First().RequestorBy,
                        Requestor_Name = x.First().RequestorByUser.Fullname,
                        Ticket_Status = x.First().IsApprove == true ? "Ticket Approve" : x.First().IsApprove == false
                        && x.First().IsReject == false ? "For Approval" : x.First().IsReject == true ? "Rejected" : "Unknown",
                        Remarks = x.First().Remarks,
                        Concern_Type = x.First().TicketType,
                        Done = x.First().IsDone,
                        GetRequestConcernByConcerns = x.Select(x => new GetRequestConcernResult.GetRequestConcernByConcern
                        {
                            TicketConcernId = x.Id,
                            UserId = x.UserId,
                            Issue_Handler = x.User.Fullname,
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
