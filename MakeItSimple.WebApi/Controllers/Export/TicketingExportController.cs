using Azure.Core;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Features.Export;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Export.ClosingTicketExport;

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


        [HttpGet("service-report")]
        public async Task<IActionResult> GeneralLedgerExport([FromQuery] ClosingTicketExportCommand command)
        {
            var filePath = $"ServiveReport {command.Date_From} - {command.Date_To}.xlsx";

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
