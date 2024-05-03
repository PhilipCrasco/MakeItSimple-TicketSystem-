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
            public int? RequestGeneratorId { get; set; }

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
                public int? RequestGeneratorId { get; set; }
                public int? DepartmentId { get; set; }
                public string Department_Name { get; set; }
                public int? ChannelId { get; set; }
                public string Channel_Name { get; set; }
                public int? UnitId { get; set; }
                public string Unit_Name { get; set; }
                public int? SubUnitId { get; set; }
                public string SubUnit_Name { get; set; }
                public string Ticket_Status { get; set; }
                //public string Concern_Status {  get; set; }
                public string Remarks { get; set; }
                public string Concern_Type { get; set; }
                public List<TicketConcern> TicketConcerns { get; set; }
                public class TicketConcern
                {
                    public int? TicketConcernId { get; set; }
                    public Guid? UserId { get; set; }
                    public string Issue_Handler { get; set; }
                    public string Concern_Description { get; set; }
                    public string Category_Description { get; set; }
                    public string SubCategory_Description { get; set; }
                    public DateTime? Start_Date { get; set; }
                    public DateTime? Target_Date { get; set; }
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
            public string Requestor { get; set; }
            public string Approver { get; set; }

            public string Role { get; set; }
            public Guid? UserId { get; set; }
            public string Concern_Status { get; set; }
            public string Search { get; set; }
            public bool? Status { get; set; }
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
                     .ThenInclude(x => x.RequestorByUser);

                if (requestConcernsQuery.Count() > 0)
                {
                    var businessUnitList = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == requestConcernsQuery.First().User.BusinessUnitId);
                    var receiverList = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                    var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                    var fillterApproval = requestConcernsQuery.Select(x => x.RequestGeneratorId);

                    if (request.Status != null)
                    {
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.IsActive == request.Status);
                    }

                    if (!request.Search.IsNullOrEmpty())
                    {
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.User.Fullname.Contains(request.Search)
                        && x.Id.ToString().Contains(request.Search));
                    }

                    if (request.Requestor == TicketingConString.Requestor)
                    {
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.UserId == request.UserId);
                    }

                    if (request.Approver == TicketingConString.Approver)
                    {
                        if (request.Role == TicketingConString.Receiver && receiverList != null)
                        {
                            if (request.UserId == receiverList.UserId)
                            {
                                //var ticketConcernApproveList = await _context.

                                var receiver = await _context.TicketConcerns.Include(x => x.RequestorByUser).Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId && x.IsApprove != true).ToListAsync();
                                var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);

                                requestConcernsQuery = requestConcernsQuery.Where(x => receiverContains.Contains(x.User.BusinessUnitId));
                            }
                            else
                            {
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestGeneratorId == null);
                            }

                        }
                        else
                        {
                            requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestGeneratorId == null);
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
                            requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestGeneratorId == null);
                        }

                    }
                }

                var results = requestConcernsQuery.Select(x => new GetRequestorTicketConcernResult
                {
                    RequestGeneratorId = x.RequestGeneratorId,
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
                        x.RequestGeneratorId,
                        DepartmentId = x.User.DepartmentId,
                        Department_Name = x.User.Department.DepartmentName,
                        UnitId = x.User.UnitId,
                        Unit_Name = x.User.Units.UnitName,
                        SubUnitId = x.User.SubUnitId,
                        SubUnit_Name = x.User.SubUnit.SubUnitName,
                        ChannelId = x.ChannelId,
                        Channel_Name = x.Channel.ChannelName,
                        Remarks = x.Remarks,
                        Concern_Type = x.TicketType,
                        Ticket_Status = x.IsApprove == true ? "Ticket Approve" : x.IsReject == true ? "Rejected" :
                         x.ConcernStatus != TicketingConString.ForApprovalTicket ? x.ConcernStatus : x.IsApprove == false
                         && x.IsReject == false ? "For Approval" : "Unknown",


                    }).Select(x => new TicketRequestConcern
                    {
                        RequestGeneratorId = x.Key.RequestGeneratorId,
                        DepartmentId = x.Key.DepartmentId,
                        Department_Name = x.Key.Department_Name,
                        UnitId = x.Key.UnitId,
                        Unit_Name = x.Key.Unit_Name,
                        SubUnitId = x.Key.SubUnitId,
                        SubUnit_Name = x.Key.SubUnit_Name,
                        ChannelId = x.Key.ChannelId,
                        Channel_Name = x.Key.Channel_Name,
                        Ticket_Status = x.Key.Ticket_Status,
                        Remarks = x.Key.Remarks,
                        Concern_Type = x.Key.Concern_Type,
                        TicketConcerns = x.Select(x => new TicketRequestConcern.TicketConcern
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

                        }).ToList(),


                    }).ToList(),

                });

                return await PagedList<GetRequestorTicketConcernResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}