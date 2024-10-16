using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.CloseReport.TicketReports;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OpenReport.OpenTicketReports;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.TransferReport.TransferTicketReports;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OnHoldReport.OnHoldTicketReport;

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

        [HttpGet("closing")]
        public async Task<IActionResult> TicketReports([FromQuery] TicketReportsQuery query)
        {
            try
            {

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


        [HttpGet("open")]
        public async Task<IActionResult> OpenTicketReports([FromQuery] OpenTicketReportsQuery query)
        {
            try
            {
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


        [HttpGet("transfer")]
        public async Task<IActionResult> TransferTicketReports([FromQuery] TransferTicketReportsQuery query)
        {
            try
            {

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

        [HttpGet("on-hold")]
        public async Task<IActionResult> OnHoldTicketReport([FromQuery] OnHoldTicketReportQuery query)
        {
            try
            {

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
