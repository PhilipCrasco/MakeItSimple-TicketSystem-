using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.TicketReports;

namespace MakeItSimple.WebApi.Controllers.Report
{
    [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ticket")]
        public async Task<IActionResult> TicketReports([FromQuery] TicketReportsQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        query.UserId = userId;
                    }
                }

                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }


    }
}
