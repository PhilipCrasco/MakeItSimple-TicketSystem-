using ClosedXML.Excel;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OpenReport.OpenTicketReports;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.OpenExport
{
    public partial class OpenTicketExport
    {

        public class Handler : IRequestHandler<OpenTicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(OpenTicketExportCommand request, CancellationToken cancellationToken)
            {
                var openTicket = await _context.TicketConcerns
                    .Include(x => x.RequestConcern)
                    .ThenInclude(x => x.Category)
                    .Where(x => x.IsApprove == true && x.IsClosedApprove != true && x.IsTransfer != true )
                    .Where(x => x.TargetDate.Value.Date >= request.Date_From.Value.Date && x.TargetDate.Value.Date <= request.Date_To.Value.Date)
                    .Select(t => new OpenTicketExportResult
                    {
                        UserId = t.UserId,
                        UnitId = t.User.UnitId,
                        TicketConcernId = t.Id,
                        Concern_Description = t.RequestConcern.Concern,
                        Requestor_Name = t.RequestorByUser.Fullname,
                        CompanyName = t.RequestConcern.Company.CompanyName,
                        Business_Unit_Name = t.RequestConcern.BusinessUnit.BusinessName,
                        Department_Name = t.RequestConcern.Department.DepartmentName,
                        Unit_Name = t.RequestConcern.Unit.UnitName,
                        SubUnit_Name = t.RequestConcern.SubUnit.SubUnitName,
                        Location_Name = t.RequestConcern.Location.LocationName,
                        Category_Description = t.RequestConcern.Category.CategoryDescription,
                        SubCategory_Description = t.RequestConcern.SubCategory.SubCategoryDescription,
                        Issue_Handler = t.User.Fullname,
                        Channel_Name = t.RequestConcern.Channel.ChannelName,
                        Target_Date = t.TargetDate.Value.Date,
                        Created_At = t.CreatedAt.Date,
                        Modified_By = t.ModifiedByUser.Fullname,
                        Updated_At = t.UpdatedAt,
                        Remarks = t.Remarks,

                    }).ToListAsync(cancellationToken);

                if (request.Unit is not null)
                {
                    openTicket = openTicket
                        .Where(x => x.UnitId == request.Unit)
                        .ToList();

                    if (request.UserId is not null)
                    {
                        openTicket = openTicket
                            .Where(x => x.UserId == request.UserId)
                            .ToList();
                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    openTicket = openTicket
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Issue_Handler.Contains(request.Search))
                        .ToList();
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Open Ticket Report");
                    var headers = new List<string>
                    {
                        "TicketConcernId",
                        "Concern Description",
                        "Requestor Name",
                        "Company Name", 
                        "Business Unit Name",
                        "Department Name",
                        "Unit Name",
                        "Sub Unit Name",
                        "Location Name",
                        "Channel Name",
                        "Category Description",
                        "Sub Category Description",
                        "Issue Handler",
                        "Target Date",
                        "Created At",
                        "Modified By",
                        "Updated At",
                        "Remarks"

                    };

                    var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                    range.Style.Fill.BackgroundColor = XLColor.LavenderPurple;
                    range.Style.Font.Bold = true;
                    range.Style.Font.FontColor = XLColor.Black;
                    range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    for (var index = 1; index <= headers.Count; index++)
                    {
                        worksheet.Cell(1, index).Value = headers[index - 1];
                    }
                    for (var index = 1; index <= openTicket.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = openTicket[index - 1].TicketConcernId;
                        row.Cell(2).Value = openTicket[index - 1].Concern_Description;
                        row.Cell(3).Value = openTicket[index - 1].Requestor_Name;
                        row.Cell(4).Value = openTicket[index - 1].CompanyName;
                        row.Cell(5).Value = openTicket[index - 1].Business_Unit_Name;
                        row.Cell(6).Value = openTicket[index - 1].Department_Name;
                        row.Cell(7).Value = openTicket[index - 1].Unit_Name;
                        row.Cell(8).Value = openTicket[index - 1].SubUnit_Name;
                        row.Cell(9).Value = openTicket[index - 1].Location_Name;
                        row.Cell(10).Value = openTicket[index - 1].Channel_Name;
                        row.Cell(11).Value = openTicket[index - 1].Category_Description;
                        row.Cell(12).Value = openTicket[index - 1].SubCategory_Description;
                        row.Cell(13).Value = openTicket[index - 1].Issue_Handler;
                        row.Cell(14).Value = openTicket[index - 1].Target_Date;
                        row.Cell(15).Value = openTicket[index - 1].Created_At;
                        row.Cell(16).Value = openTicket[index - 1].Modified_By;
                        row.Cell(17).Value = openTicket[index - 1].Updated_At;
                        row.Cell(18).Value = openTicket[index - 1].Remarks;

                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"OpenTicketReport {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;

            }
        }
    }
}
