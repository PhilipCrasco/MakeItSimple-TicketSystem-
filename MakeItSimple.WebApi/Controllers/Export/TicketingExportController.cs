using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Excel;
using MakeItSimple.WebApi.DataAccessLayer.Features.Export.TransferExport;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Export.ClosingExport.ClosingTicketExport;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Export.OpenExport.OpenTicketExport;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Export.TransferExport.TransferTicketExport;

namespace MakeItSimple.WebApi.Controllers.Export
{
    [Route("api/export")]
    [ApiController]
    public class TicketingExportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketingExportController(IMediator mediator )
        {
            _mediator = mediator;
        }


        [HttpGet("closing")]
        public async Task<IActionResult> ClosingTicketExport([FromQuery] ClosingTicketExportCommand command)
        {
            var filePath = $"ClosingTicketReports {command.Date_From:MM-dd-yyyy} - {command.Date_To:MM-dd-yyyy}.xlsx";

            try
            {
                await _mediator.Send(command);
                var memory = new MemoryStream();
                await using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filePath);
                System.IO.File.Delete(filePath);
                return result;

            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }

        }


        [HttpGet("open")]
        public async Task<IActionResult> OpenTicketExport([FromQuery] OpenTicketExportCommand command)
        {
            var filePath = $"OpenTicketReport {command.Date_From:MM-dd-yyyy} - {command.Date_To:MM-dd-yyyy}.xlsx";

            try
            {
                await _mediator.Send(command);
                var memory = new MemoryStream();
                await using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filePath);
                System.IO.File.Delete(filePath);
                return result;

            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }

        }

        [HttpGet("transfer")]
        public async Task<IActionResult> TransferTicketExport([FromQuery] TransferTicketExportCommand command)
        {
            var filePath = $"TransferTicketReport {command.Date_From:MM-dd-yyyy} - {command.Date_To:MM-dd-yyyy}.xlsx";

            try
            {
                await _mediator.Send(command);
                var memory = new MemoryStream();
                await using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filePath);
                System.IO.File.Delete(filePath);
                return result;

            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }

        }


    }
}
