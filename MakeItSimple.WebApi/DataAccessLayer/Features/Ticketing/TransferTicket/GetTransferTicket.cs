using Google.Protobuf.WellKnownTypes;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using MoreLinq;
using System.Transactions;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransferTicket.GetTransferTicketResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class GetTransferTicket
    {

        public class GetTransferTicketResult
        {

            public int ? TicketGeneratorId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Channel_Name { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public bool IsActive { get; set; }
            public string  Transfer_By { get; set; }
            public DateTime ? Transfer_At { get; set; }
            public string Transfer_Status { get; set; }
            public string Transfer_Remarks { get; set; }
            public string RejectTransfer_By { get; set; }
            public DateTime? RejectTransfer_At { get; set; }
            public string Reject_Remarks { get; set; }

            public List<GetTransferTicketConcern> GetTransferTicketConcerns { get; set; }

            public class GetTransferTicketConcern
            {

                public int TransferTicketConcernId { get; set; }
                public int TicketConcernId { get; set; }
                public string Concern_Details { get; set; }
                public string Category_Description { get; set; }
                public string SubCategoryDescription { get; set; }

                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }

                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }

                public DateTime ? Start_Date { get; set; }
                public DateTime ? Target_Date { get; set; }

            }


        }

        public class GetTransferTicketQuery : UserParams, IRequest<PagedList<GetTransferTicketResult>>
        {
            public Guid ? UserId { get; set; }
            //public string Approval { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public bool ? IsTransfer { get; set; }
            public bool ? IsReject { get; set; }
            public string Search { get; set; }
            public bool ? Status { get; set; }


            
        }

        public class Handler : IRequestHandler<GetTransferTicketQuery, PagedList<GetTransferTicketResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetTransferTicketResult>> Handle(GetTransferTicketQuery request, CancellationToken cancellationToken)
            {


                IQueryable<TransferTicketConcern> transferTicketQuery = _context.TransferTicketConcerns
                    .Include(x => x.TicketConcern)
                    .Include(x => x.RequestGenerator)
                    .ThenInclude(x => x.ApproverTicketings)
                    .Include(x => x.AddedByUser)
                    .Include(x => x.User)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.TransferByUser);


                var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                var fillterApproval = transferTicketQuery.Select(x => x.RequestGeneratorId);


                if (!string.IsNullOrEmpty(request.Search))
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.User.Fullname.Contains(request.Search)
                    || x.User.EmpId.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.IsActive == request.Status);
                }

                if (request.IsTransfer != null)
                {

                    transferTicketQuery = transferTicketQuery.Where(x => x.IsTransfer == request.IsTransfer);
                }

                if (request.IsReject != null)
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.IsRejectTransfer == request.IsReject);
                }

                if (request.UserType == TicketingConString.Approval)
                {

                    if (request.UserId != null && TicketingConString.Approver == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => x.UserId == userApprover.Id).ToListAsync();
                        var approvalLevelList = approverTransactList.Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null).ToList();
                        var userRequestIdApprovalList = approvalLevelList.Select(x => x.TicketGeneratorId);
                        var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                        transferTicketQuery = transferTicketQuery.Where(x => userIdsInApprovalList.Contains(x.TicketApprover) && userRequestIdApprovalList.Contains(x.TicketGeneratorId));

                    }
                    else
                    {
                        transferTicketQuery = transferTicketQuery.Where(x => x.TicketGeneratorId == null);
                    }

                }

                if(request.UserType == TicketingConString.Users)
                {
                   transferTicketQuery = transferTicketQuery.Where(x => x.AddedByUser.Id == request.UserId); 
                }

                var results = transferTicketQuery.GroupBy(x => x.TicketGeneratorId).Select(x => new GetTransferTicketResult
                {
                    TicketGeneratorId = x.Key,
                    Department_Code = x.First().User.Department.DepartmentCode,
                    Department_Name = x.First().User.Department.DepartmentName,
                    Unit_Code = x.First().User.Units.UnitCode,
                    Unit_Name = x.First().User.Units.UnitName,
                    SubUnit_Code = x.First().User.SubUnit.SubUnitCode,
                    SubUnit_Name = x.First().User.SubUnit.SubUnitName,
                    Channel_Name = x.First().Channel.ChannelName,
                    EmpId = x.First().User.EmpId,
                    Fullname = x.First().User.Fullname,
                    IsActive = x.First().IsActive,
                    Transfer_By = x.First().TransferByUser.Fullname,
                    Transfer_At = x.First().TransferAt,
                    Transfer_Status = x.First().IsTransfer == false && x.First().IsRejectTransfer == false ? "For Transfer Approval" : x.First().IsTransfer == true 
                    && x.First().IsRejectTransfer == false ? "Transfer Approve" : x.First().IsRejectTransfer == true ? "Transfer Reject" : "Unknown",
                    Transfer_Remarks = x.First().TransferRemarks,
                    RejectTransfer_By = x.First().RejectTransferByUser.Fullname,
                    RejectTransfer_At = x.First().RejectTransferAt,
                    Reject_Remarks = x.First().RejectRemarks,

                    GetTransferTicketConcerns = x.Select(x => new GetTransferTicketResult.GetTransferTicketConcern
                    {
                        TransferTicketConcernId = x.Id,
                        TicketConcernId = x.TicketConcernId,
                        Concern_Details = x.ConcernDetails,
                        Category_Description = x.Category.CategoryDescription,
                        SubCategoryDescription = x.SubCategory.SubCategoryDescription,       
                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.CreatedAt,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt,
                        Start_Date = x.StartDate,
                        Target_Date = x.TargetDate

                    }).ToList()

                });


                return await PagedList<GetTransferTicketResult>.CreateAsync(results, request.PageNumber , request.PageSize);
            }
        }
    }
}
