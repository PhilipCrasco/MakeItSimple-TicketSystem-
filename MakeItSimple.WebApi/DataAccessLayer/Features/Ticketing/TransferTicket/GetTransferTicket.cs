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
            public int TicketConcernId { get; set; }
            public int TransferTicketConcernId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Channel_Name { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public bool IsActive { get; set; }

            public string Added_By {  get; set; }
            public DateTime Created_At { get; set; }

            public string Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }

            public string  Transfer_By { get; set; }
            public DateTime ? Transfer_At { get; set; }
            public string TransferStatus { get; set; }
            public string TransferRemarks { get; set; }

            public List<GetTransferTicketConcern> GetTransferTicketConcerns { get; set; }

            public class GetTransferTicketConcern
            {
                public int TransferTicketConcernId { get; set; }
                public string Concern_Description { get; set; }
                public string Category_Description { get; set; }
                public string SubCategoryDescription { get; set; }
                public DateTime ? Start_Date { get; set; }
                public DateTime ? Target_Date { get; set; }

            }


        }

        public class GetTransferTicketQuery : UserParams, IRequest<PagedList<GetTransferTicketResult>>
        {
            public Guid ? UserId { get; set; }
            public bool ? IsTransfer { get; set; }
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
                    .Include(x => x.AddedByUser)
                    .Include(x => x.User)
                    .Include(x => x.SubUnit)
                    .Include(x => x.Channel)
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.TransferByUser);

                var channeluserExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                if (channeluserExist != null)
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.User.Fullname == channeluserExist.Fullname);
                }

                if(!string.IsNullOrEmpty(request.Search))
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.User.Username.Contains(request.Search)
                    || x.SubUnit.SubUnitName.Contains(request.Search)
                    || x.Channel.ChannelName.Contains(request.Search));
                }

                if(request.Status != null)
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.IsActive == request.Status);
                }

                if(request.IsTransfer != null)
                {
                    transferTicketQuery = transferTicketQuery.Where(x => x.IsTransfer == request.IsTransfer);
                }


                var results = transferTicketQuery.GroupBy(x => x.Id).Select(x => new GetTransferTicketResult
                {
                    TicketConcernId = x.First().TicketConcernId,
                    TransferTicketConcernId = x.First().Id,
                    Department_Code = x.First().Department.DepartmentCode,
                    Department_Name = x.First().Department.DepartmentName,
                    SubUnit_Code = x.First().SubUnit.SubUnitCode,
                    SubUnit_Name = x.First().SubUnit.SubUnitName,
                    Channel_Name = x.First().Channel.ChannelName,
                    EmpId = x.First().User.EmpId,
                    Fullname = x.First().User.Fullname,
                    IsActive = x.First().IsActive,
                    Added_By = x.First().AddedByUser.Fullname,
                    Created_At = x.First().CreatedAt,
                    Modified_By = x.First().ModifiedByUser.Fullname,
                    Updated_At = x.First().UpdatedAt,
                    Transfer_By = x.First().TransferByUser.Fullname,
                    Transfer_At = x.First().TransferAt,
                    TransferStatus = x.First().IsTransfer == true ? "Transfer Approve" : x.First().IsTransfer != true && x.First().IsActive == false ? "Transfer Ticket Rejected" : "Tranfer Ticket for Approval",
                    TransferRemarks = x.First().TransferRemarks,
                    GetTransferTicketConcerns = x.Select(x => new GetTransferTicketResult.GetTransferTicketConcern
                    {
                        TransferTicketConcernId = x.Id,
                        Concern_Description = x.ConcernDetails,
                        Category_Description = x.Category.CategoryDescription,
                        SubCategoryDescription = x.SubCategory.SubCategoryDescription,
                        Start_Date = x.StartDate,
                        Target_Date = x.TargetDate
                    }).ToList()

                });


                return await PagedList<GetTransferTicketResult>.CreateAsync(results, request.PageNumber , request.PageSize);
            }
        }
    }
}
