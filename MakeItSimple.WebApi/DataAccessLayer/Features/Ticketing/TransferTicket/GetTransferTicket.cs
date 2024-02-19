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

            public int ? RequestGeneratorId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
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
            public  Guid ? UserApproverId { get; set; }
            public string Approval { get; set; }
            public bool ? IsTransfer { get; set; }
            public bool ? IsReject { get; set; }
            public string Search { get; set; }
            public bool ? Status { get; set; }

            public string Role { get ; set; }

            
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
                    .Include(x => x.SubUnit)
                    .Include(x => x.Channel)
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.TransferByUser);


                var channeluserExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserApproverId, cancellationToken);

                if (request.UserApproverId != null)
                {
                    var approverTransactList = await _context.ApproverTicketings.Where(x => x.UserId == userApprover.Id).ToListAsync();
                    var approvalLevelList = approverTransactList.Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null).ToList();
                    var userRequestIdApprovalList = approvalLevelList.Select(x => x.RequestGeneratorId);
                    var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                    transferTicketQuery = transferTicketQuery.Where(x => userIdsInApprovalList.Contains(x.TicketApprover) && userRequestIdApprovalList.Contains(x.RequestGeneratorId));

                }
                
                var fillterApproval = transferTicketQuery.Select(x => x.RequestGeneratorId);

                if(request.Approval == TicketingConString.Approval)
                {
                    
                    var approverTransactList = await _context.ApproverTicketings.Where(x => fillterApproval.Contains(x.RequestGeneratorId) && x.IsApprove == null).ToListAsync();

                    if(approverTransactList != null && approverTransactList.Any())
                    {
                        var generatedIdInApprovalList = approverTransactList.Select(approval => approval.RequestGeneratorId);
                        transferTicketQuery = transferTicketQuery.Where(x => !generatedIdInApprovalList.Contains(x.RequestGeneratorId));
                    }

                }
 
                if (channeluserExist != null)
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.AddedByUser.Fullname == channeluserExist.Fullname);
                }

                if(!string.IsNullOrEmpty(request.Search))
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.User.Fullname.Contains(request.Search)
                    || x.User.EmpId.Contains(request.Search));
                }

                if(request.Status != null)
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.IsActive == request.Status);
                }

                if(request.IsTransfer != null)
                {

                    transferTicketQuery = transferTicketQuery.Where(x => x.IsTransfer == request.IsTransfer);
                }

                if(request.IsReject != null)
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.IsRejectTransfer == request.IsReject);
                }

                var results = transferTicketQuery.GroupBy(x => x.RequestGeneratorId).Select(x => new GetTransferTicketResult
                {
                    RequestGeneratorId = x.Key,
                    Department_Code = x.First().Department.DepartmentCode,
                    Department_Name = x.First().Department.DepartmentName,
                    SubUnit_Code = x.First().SubUnit.SubUnitCode,
                    SubUnit_Name = x.First().SubUnit.SubUnitName,
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
