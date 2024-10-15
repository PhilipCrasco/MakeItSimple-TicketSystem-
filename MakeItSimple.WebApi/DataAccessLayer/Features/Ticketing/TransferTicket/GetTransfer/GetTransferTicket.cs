using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransfer.GetTransferTicket.GetTransferTicketResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransfer
{
    public partial class GetTransferTicket
    {

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
                    .ThenInclude(x => x.RequestConcern)
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.TransferByUser);

                if (transferTicketQuery.Any())
                {
                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions

                        }).ToListAsync();

                    var issueHandlerPermissionList = allUserList
                        .Where(x => x.Permissions
                        .Contains(TicketingConString.IssueHandler))
                        .Select(x => x.UserRoleName)
                        .ToList();

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        transferTicketQuery = transferTicketQuery
                            .Where(x => x.TicketConcern.User.Fullname
                            .Contains(request.Search)
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

                    if(!string.IsNullOrEmpty(request.UserType))
                    {
                        if (request.UserType == TicketingConString.Approver)
                        {
                            transferTicketQuery = transferTicketQuery.Where(x => x.TransferTo == request.UserId);
                        }
                        else if (request.UserType == TicketingConString.IssueHandler)
                        {
                            transferTicketQuery = transferTicketQuery.Where(x => x.AddedByUser.Id == request.UserId);
                        }

                    }

                }

                var results = transferTicketQuery
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new GetTransferTicketResult
                    {
                        TicketConcernId = x.TicketConcernId,
                        TransferTicketId = x.Id,
                        Department_Code = x.TicketConcern.User.Department.DepartmentCode,
                        Department_Name = x.TicketConcern.User.Department.DepartmentName,
                        ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                        Channel_Name = x.TicketConcern.RequestConcern.Channel.ChannelName,
                        UserId = x.TicketConcern.UserId,
                        Fullname = x.TicketConcern.User.Fullname,
                        Concern_Details = x.TicketConcern.RequestConcern.Concern,
                        Category_Description = x.TicketConcern.RequestConcern.Category.CategoryDescription,
                        SubCategory_Description = x.TicketConcern.RequestConcern.SubCategory.SubCategoryDescription,
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
