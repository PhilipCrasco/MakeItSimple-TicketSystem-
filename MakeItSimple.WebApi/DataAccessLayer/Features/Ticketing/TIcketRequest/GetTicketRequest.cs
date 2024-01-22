using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.GetTicketRequest.GetTicketRequestResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest
{
    public class GetTicketRequest
    {
        public class GetTicketRequestResult
        {
            public int? RequestGeneratedId { get; set; }
            public int DepartmentId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public int SubUnitId { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }

            public int ChannelId { get; set; }
            public string Channel_Name { get; set; }

            public Guid? UserId { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }


            public string TicketStatus { get; set; }

            public string Approved_By { get; set; }
            public DateTime? Approve_At { get; set; }

            public string RejectRemarks { get; set; }

            public List<TicketConcerns> TicketConcern { get; set; }

            public class TicketConcerns
            {
                public int TicketConcernId { get; set; }
                public string Concern_Description { get; set; }

                public int CategoryId { get; set; }

                public string Category_Description { get; set; }
                public int SubCategoryId { get; set; }
               public string SubCategoryDescription { get; set; }

                public DateTime Start_Date { get; set; }

                public DateTime Target_Date { get; set; }

                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; } 

                public bool IsActive { get; set; }

            }


        }

        public class GetTicketRequestQuery : UserParams, IRequest<PagedList<GetTicketRequestResult>>
        {

            public Guid? UserId { get; set; }

            public string Search { get; set; }

            public bool? Status { get; set; }

            public bool? Approval { get; set; }

            public bool? Reject { get; set; }

        }

        public class Handler : IRequestHandler<GetTicketRequestQuery, PagedList<GetTicketRequestResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetTicketRequestResult>> Handle(GetTicketRequestQuery request, CancellationToken cancellationToken)
            {

                IQueryable<TicketConcern> ticketQuery = _context.TicketConcerns
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ApprovedByUser)
                    .Include(x => x.TransferByUser)
                    .Include(x => x.RequestGenerator)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.Department)
                    .Include(x => x.SubUnit)
                    .Include(x => x.Channel)
                    .Include(x => x.User)
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory);



                var channeluserExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                if (channeluserExist != null)
                {
                    ticketQuery = ticketQuery.Where(x => x.User.Fullname == channeluserExist.Fullname);
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    ticketQuery = ticketQuery.Where(x => x.Department.DepartmentName.Contains(request.Search)
                    || x.SubUnit.SubUnitName.Contains(request.Search)
                    || x.User.Fullname.Contains(request.Search));

                }

                if (request.Status != null)
                {
                    ticketQuery = ticketQuery.Where(x => x.IsActive == request.Status);
                }

                if (request.Approval != null)
                {
                    ticketQuery = ticketQuery.Where(x => x.IsApprove == request.Approval);
                }

                if (request.Reject != null)
                {
                    ticketQuery = ticketQuery.Where(x => x.IsReject == request.Reject);
                }



                var result =  ticketQuery.Include(x => x.RequestGenerator).ThenInclude(x => x.TicketAttachments)
                    .GroupBy(x => x.RequestGeneratorId).Select(x => new GetTicketRequestResult
                    {
                        RequestGeneratedId = x.Key,
                        DepartmentId = x.First().DepartmentId,
                        Department_Code = x.First().Department.DepartmentCode,
                        Department_Name = x.First().Department.DepartmentName,
                        SubUnitId = x.First().SubUnitId,
                        SubUnit_Code = x.First().SubUnit.SubUnitCode,
                        SubUnit_Name = x.First().SubUnit.SubUnitName,
                        ChannelId = x.First().ChannelId,
                        Channel_Name = x.First().Channel.ChannelName,
                        UserId = x.First().UserId,
                        EmpId = x.First().User.EmpId,
                        Fullname = x.First().User.Fullname,
                        TicketStatus = x.First().IsApprove == true ? "Ticket Approve" : x.First().IsApprove == false
                    && x.First().IsReject == false ? "For Approval" : x.First().IsReject == true ? "Rejected" : "Unknown",

                        Approved_By = x.First().ApprovedByUser.Fullname,
                        Approve_At = x.First().ApprovedAt,
                        RejectRemarks = x.First().RejectRemarks,
                        TicketConcern = x.Select(x => new TicketConcerns
                        {
                            TicketConcernId = x.Id,
                            Concern_Description = x.ConcernDetails,
                            CategoryId = x.CategoryId,
                            Category_Description = x.Category.CategoryDescription,
                            SubCategoryId = x.SubCategoryId,
                            SubCategoryDescription = x.SubCategory.SubCategoryDescription,
                            Start_Date = x.StartDate,
                            Target_Date = x.TargetDate,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Modified_By = x.ModifiedByUser.Fullname,
                            Updated_At = x.UpdatedAt,
                            IsActive = x.IsActive,

                        }).ToList(),


                    });

                return await PagedList<GetTicketRequestResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }

}
