using ClosedXML.Excel;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OpenTicketReports;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export
{
    public class OpenTicketExport
    {
        public class OpenTicketExportCommand : IRequest<Unit>
        {
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }

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
                    .Where(x => x.IsApprove == true && x.IsClosedApprove != true && x.IsTransfer != true)
                    .Select(t => new OpenTicketReportsResult
                    {
                        TicketConcernId = t.Id,
                        Concern_Description = t.ConcernDetails,
                        Requestor_Name = t.RequestorByUser.Fullname,
                        Department_Name = t.RequestorByUser.Department.DepartmentName,
                        Issue_Handler = t.User.Fullname,
                        Unit_Name = t.User.Units.UnitName,
                        SubUnit_Name = t.User.SubUnit.SubUnitName,
                        Channel_Name = t.Channel.ChannelName,
                        Category_Description = t.Category.CategoryDescription,
                        SubCategory_Description = t.SubCategory.SubCategoryDescription,
                        Start_Date = t.StartDate,
                        Target_Date = t.TargetDate,
                        Created_At = t.CreatedAt,
                        Modified_By = t.ModifiedByUser.Fullname,
                        Updated_At = t.UpdatedAt,
                        Remarks = t.Remarks,

                    }).ToListAsync(cancellationToken);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Service Report");
                    var headers = new List<string>
                    {
                        "TicketConcernId",
                        "Concern Description",
                        "Requestor Name",
                        "Department Name",
                        "Issue Handler",
                        "Unit Name",
                        "Sub Unit Name",
                        "Channel Name",
                        "Category Description",
                        "Sub Category Description",
                        "Start Date",
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
                        row.Cell(4).Value = openTicket[index - 1].Department_Name;
                        row.Cell(5).Value = openTicket[index - 1].Issue_Handler;
                        row.Cell(6).Value = openTicket[index - 1].Unit_Name;
                        row.Cell(7).Value = openTicket[index - 1].SubUnit_Name;
                        row.Cell(8).Value = openTicket[index - 1].Category_Description;
                        row.Cell(9).Value = openTicket[index - 1].SubCategory_Description;
                        row.Cell(10).Value = openTicket[index - 1].Start_Date;
                        row.Cell(11).Value = openTicket[index - 1].Target_Date;
                        row.Cell(12).Value = openTicket[index - 1].Created_At;
                        row.Cell(13).Value = openTicket[index - 1].Modified_By;
                        row.Cell(14).Value = openTicket[index - 1].Updated_At;
                        row.Cell(15).Value = openTicket[index - 1].Remarks;

                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"ServiceReport {request.Date_From} - {request.Date_To}.xlsx");

                }



                return Unit.Value;

            }
        }
    }
}
