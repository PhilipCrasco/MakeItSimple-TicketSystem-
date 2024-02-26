using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.GetReTicket;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class GetClosingTicket
    {

        public class GetClosingTicketResults
        {
            public int? RequestGeneratorId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Channel_Name { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public bool IsActive { get; set; }

            //public int ? Delay_Days { get; set; }

            public string Closed_By { get; set; }
            public DateTime? Closed_At { get; set; }
            public string Closed_Status { get; set; }
            public string Closed_Remarks { get; set; }
            public string RejectClosed_By { get; set; }
            public DateTime? RejectClosed_At { get; set; }
            public string Reject_Remarks { get; set; }

            public int RequestCount { get; set; }

            public List<GetClosedTicketConcern> GetClosedTicketConcerns {  get; set; }

            public class GetClosedTicketConcern
            {
                public int ReTicketConcernId { get; set; }
                public int TicketConcernId { get; set; }
                public string Concern_Details { get; set; }
                public string Category_Description { get; set; }
                public string SubCategoryDescription { get; set; }
                public int? Delay_Days { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }

                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }

                public DateTime? Start_Date { get; set; }
                public DateTime? Target_Date { get; set; }
            }
            
        }


        public class GetClosingTicketQuery : UserParams , IRequest<PagedList<GetClosingTicketResults>>
        {
            public Guid? UserId { get; set; }
            public string Approval { get; set; }
            public string Users { get; set; }
            public string Role { get; set; }

            public string Search { get; set; }
            public bool? IsClosed { get; set; }
            public bool? IsReject { get; set; }
            public Guid? UserApproverId { get; set; }


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
                    .Include(x => x.Department)
                    .Include(x => x.SubUnit)
                    .Include(x => x.Channel)
                    .Include(x => x.User)
                    .Include(x => x.RejectClosedByUser)
                    .Include(x => x.ClosedByUser);

                var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                var fillterApproval = closingTicketsQuery.Select(x => x.RequestGeneratorId);

                if (TicketingConString.Approval == request.Approval)
                {

                    if (request.UserId != null && TicketingConString.Approver == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => x.UserId == userApprover.Id).ToListAsync();
                        var approvalLevelList = approverTransactList.Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null).ToList();
                        var userRequestIdApprovalList = approvalLevelList.Select(x => x.RequestGeneratorId);
                        var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                        closingTicketsQuery = closingTicketsQuery.Where(x => userIdsInApprovalList.Contains(x.TicketApprover) && userRequestIdApprovalList.Contains(x.RequestGeneratorId));

                    }

                    else if (TicketingConString.Admin == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => fillterApproval.Contains(x.RequestGeneratorId) && x.IsApprove == null).ToListAsync();

                        if (approverTransactList != null && approverTransactList.Any())
                        {
                            var generatedIdInApprovalList = approverTransactList.Select(approval => approval.RequestGeneratorId);
                            closingTicketsQuery = closingTicketsQuery.Where(x => !generatedIdInApprovalList.Contains(x.RequestGeneratorId));
                        }

                    }
                    else
                    {
                        closingTicketsQuery = closingTicketsQuery.Where(x => x.RequestGeneratorId == null);
                    }

                }


                if (TicketingConString.Users == request.Users)
                {
                     closingTicketsQuery = closingTicketsQuery.Where(x => x.AddedByUser.Id == request.UserId);
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    closingTicketsQuery = closingTicketsQuery.Where(x => x.User.Fullname.Contains(request.Search)
                                    || x.User.EmpId.Contains(request.Search));
                }

                if (request.IsReject != null)
                {
                    closingTicketsQuery = closingTicketsQuery.Where(x => x.IsRejectClosed == request.IsReject);
                }

                if (request.IsClosed != null)
                {
                    closingTicketsQuery = closingTicketsQuery.Where(x => x.IsClosing == request.IsClosed );
                }


                var distictQuery = closingTicketsQuery.Select(x => x.RequestGeneratorId).Distinct();

                var results = closingTicketsQuery.GroupBy(x => x.RequestGeneratorId)
                    .Select(x => new GetClosingTicketResults
                    {
                        RequestGeneratorId = x.Key, 
                        Department_Code = x.First().Department.DepartmentCode,
                        Department_Name = x.First().Department.DepartmentName,
                        SubUnit_Code = x.First().SubUnit.SubUnitCode,
                        SubUnit_Name = x.First().SubUnit.SubUnitName,
                        Channel_Name = x.First().Channel.ChannelName,
                        EmpId = x.First().User.EmpId,   
                        Fullname = x.First().User.Fullname,
                        IsActive = x.First().User.IsActive,
                        RejectClosed_By = x.First().RejectClosedByUser.Fullname,
                        RejectClosed_At= x.First().RejectClosedAt,
                        Reject_Remarks = x.First().RejectRemarks,
                        Closed_By = x.First().ClosedByUser.Fullname,
                        Closed_At = x.First().ClosingAt,

                        RequestCount = distictQuery.Count(),

                        Closed_Remarks = x.First().ClosingRemarks,
                        GetClosedTicketConcerns = x.Select(x => new GetClosingTicketResults.GetClosedTicketConcern
                        {
                           
                            ReTicketConcernId = x.Id,
                            TicketConcernId = x.TicketConcernId,
                            Concern_Details = x.ConcernDetails,
                            Category_Description = x.Category.CategoryDescription,
                            SubCategoryDescription = x.SubCategory.SubCategoryDescription,
                            Start_Date = x.StartDate,
                            Target_Date = x.TargetDate,
                            Delay_Days = x.TargetDate < dateToday && x.ClosingAt == null ? EF.Functions.DateDiffDay(x.TargetDate, dateToday)
                            : x.TargetDate < x.ClosingAt && x.ClosingAt != null ? EF.Functions.DateDiffDay(x.TargetDate, x.ClosingAt) : 0,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Updated_At = x.UpdatedAt,
                            Modified_By = x.ModifiedByUser.Fullname

                        }).ToList(),
                    });

                return await PagedList<GetClosingTicketResults>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }



}
