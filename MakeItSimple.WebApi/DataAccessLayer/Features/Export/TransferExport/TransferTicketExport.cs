using ClosedXML.Excel;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.TransferExport
{
    public partial class TransferTicketExport
    {

        public class Handler : IRequestHandler<TransferTicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(TransferTicketExportCommand request, CancellationToken cancellationToken)
            {
                var _transferQuery = await _context.TransferTicketConcerns
                    .AsNoTracking()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.TransferByUser)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .Include(x => x.TicketConcern)
                    .Where(x => x.IsTransfer == true && x.TicketConcern.UserId != null)
                    .Where(x => x.TransferAt >= request.Date_From && x.TransferAt < request.Date_To)
                    .Select(x => new TransferTicketExportResult
                    {
                        Unit = x.TransferByUser.UnitId,
                        UserId = x.TransferBy,
                        TicketConcernId = x.TicketConcernId,
                        TransferTicketId = x.Id,
                        Concern_Details = x.TicketConcern.RequestConcern.Concern,
                        Transfered_By = x.TransferByUser.Fullname,
                        Transfered_To = x.TransferToUser.Fullname,
                        Current_Target_Date = x.Current_Target_Date.Value.Date,
                        Target_Date = x.TicketConcern.TargetDate.Value.Date,
                        Transfer_At = x.TicketConcern.TransferAt,
                        Transfer_Remarks = x.TransferRemarks,
                        Remarks = x.TransferRemarks,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt,

                    }).ToListAsync(cancellationToken);


                if (request.Unit is not null)
                {
                    _transferQuery = _transferQuery.Where(x => x.Unit == request.Unit)
                        .ToList();

                    if (request.UserId is not null)
                    {
                        _transferQuery = _transferQuery.Where(x => x.UserId == request.UserId)
                            .ToList();
                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    _transferQuery = _transferQuery
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Transfered_By.Contains(request.Search))
                        .ToList();
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Transfer Ticket Report");
                    var headers = new List<string>
                    {
                        "TicketConcernId",
                        "Concern Details",
                        "Transfer By",
                        "Transfer To",
                        "Current Target Date",
                        "Target Date",
                        "Transfer At",
                        "Transfer Remarks",
                        "Remarks",
                        "Modified By",
                        "Updated At"

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
                    for (var index = 1; index <= _transferQuery.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = _transferQuery[index - 1].TicketConcernId;
                        row.Cell(2).Value = _transferQuery[index - 1].Concern_Details;
                        row.Cell(3).Value = _transferQuery[index - 1].Transfered_By;
                        row.Cell(4).Value = _transferQuery[index - 1].Transfered_To;
                        row.Cell(5).Value = _transferQuery[index - 1].Current_Target_Date;
                        row.Cell(6).Value = _transferQuery[index - 1].Target_Date;
                        row.Cell(7).Value = _transferQuery[index - 1].Transfer_At;
                        row.Cell(8).Value = _transferQuery[index - 1].Transfer_Remarks;
                        row.Cell(9).Value = _transferQuery[index - 1].Remarks;
                        row.Cell(10).Value = _transferQuery[index - 1].Modified_By;
                        row.Cell(11).Value = _transferQuery[index - 1].Updated_At;

                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"TransferTicketReport {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;

            }

        }
    }
}
