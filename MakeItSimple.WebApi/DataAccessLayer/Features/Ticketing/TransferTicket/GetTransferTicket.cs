using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransferTicket.GetTransferTicketResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class GetTransferTicket
    {

        public class GetTransferTicketResult
        {

            public int? TicketConcernId { get; set; }
            public int? TransferTicketId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }
            public string Concern_Details { get; set; }
            public string Category_Description { get; set; }
            public string SubCategory_Description { get; set; }
            public DateTime? Start_Date { get; set; }
            public DateTime? Target_Date { get; set; }
            public bool IsActive { get; set; }
            public string Transfer_By { get; set; }
            public DateTime? Transfer_At { get; set; }
            public string Transfer_Status { get; set; }
            public string Transfer_Remarks { get; set; }
            public string RejectTransfer_By { get; set; }
            public DateTime? RejectTransfer_At { get; set; }
            public string Reject_Remarks { get; set; }
            public string Remarks { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

            public List<TransferAttachment> TransferAttachments { get; set; }

            public class TransferAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public string Attachment { get; set; }
                public string FileName { get; set; }
                public decimal? FileSize { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }
            }

        }

        public class GetTransferTicketQuery : UserParams, IRequest<PagedList<GetTransferTicketResult>>
        {
            public Guid? UserId { get; set; }
            //public string Approval { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public bool? IsTransfer { get; set; }
            public bool? IsReject { get; set; }
            public string Search { get; set; }
            public bool? Status { get; set; }



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
                    .AsNoTracking()
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .ThenInclude(x => x.Department)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.Channel)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.SubCategories)
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.TransferByUser);

                var allUserList = await _context.UserRoles.AsNoTracking().ToListAsync();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();


                var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();



                if (!string.IsNullOrEmpty(request.Search))
                {
                    transferTicketQuery = transferTicketQuery
                        .Where(x => x.TicketConcern.User.Fullname.Contains(request.Search)
                    || x.TicketConcernId.ToString().Contains(request.Search));
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
                            .ToListAsync();

                        var approvalLevelList = approverTransactList
                            .Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null)
                            .ToList();

                        var userRequestIdApprovalList = approvalLevelList.Select(x => x.TransferTicketConcernId);
                        var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);

                        transferTicketQuery = transferTicketQuery
                            .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                            && userRequestIdApprovalList.Contains(x.Id));
                    }
                    else
                    {
                        return new PagedList<GetTransferTicketResult>(new List<GetTransferTicketResult>(), 0, request.PageNumber, request.PageSize);
                    }

                }

                if (request.UserType == TicketingConString.IssueHandler)
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.AddedByUser.Id == request.UserId);
                }

                var results = transferTicketQuery.Select(x => new GetTransferTicketResult
                {
                    TicketConcernId = x.TicketConcernId,
                    TransferTicketId = x.Id,
                    Department_Code = x.TicketConcern.User.Department.DepartmentCode,
                    Department_Name = x.TicketConcern.User.Department.DepartmentName,
                    ChannelId = x.TicketConcern.ChannelId,
                    Channel_Name = x.TicketConcern.Channel.ChannelName,
                    UserId = x.TicketConcern.UserId,
                    Fullname = x.TicketConcern.User.Fullname,
                    Concern_Details = x.TicketConcern.ConcernDetails,
                    Category_Description = x.TicketConcern.Category.CategoryDescription,
                    SubCategory_Description = x.TicketConcern.SubCategory.SubCategoryDescription,
                    Start_Date = x.TicketConcern.StartDate,
                    Target_Date = x.TicketConcern.TargetDate,
                    IsActive = x.IsActive,
                    Transfer_By = x.TransferByUser.Fullname,
                    Transfer_At = x.TransferAt,
                    Transfer_Status = x.IsTransfer == false && x.IsRejectTransfer == false ? "For Transfer Approval"
                                    : x.IsTransfer == true && x.IsRejectTransfer == false ? "Transfer Approve"
                                    : x.IsRejectTransfer == true ? "Transfer Reject" : "Unknown",

                    Transfer_Remarks = x.TransferRemarks,
                    RejectTransfer_By = x.RejectTransferByUser.Fullname,
                    RejectTransfer_At = x.RejectTransferAt,
                    Reject_Remarks = x.RejectRemarks,
                    Remarks = x.Remarks,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    TransferAttachments = x.TicketAttachments
                    .Select(x => new TransferAttachment
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

                return await PagedList<GetTransferTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
