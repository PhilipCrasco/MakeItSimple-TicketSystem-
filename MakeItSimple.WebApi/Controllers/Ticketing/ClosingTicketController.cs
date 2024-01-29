using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosingTicket;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosingTicket.ForClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosingTicket.GetOpenTicket;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/ClosingTicket")]
    [ApiController]
    public class ClosingTicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClosingTicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetOpenTicket")]
        public async Task<IActionResult> GetOpenTicket([FromQuery] GetOpenTicketQuery query)
        {
            try
            {
                var closingTicket = await _mediator.Send(query);

                Response.AddPaginationHeader(

                closingTicket.CurrentPage,
                closingTicket.PageSize,
                closingTicket.TotalCount,
                closingTicket.TotalPages,
                closingTicket.HasPreviousPage,
                closingTicket.HasNextPage

                );

                var result = new
                {
                    closingTicket,
                    closingTicket.CurrentPage,
                    closingTicket.PageSize,
                    closingTicket.TotalCount,
                    closingTicket.TotalPages,
                    closingTicket.HasPreviousPage,
                    closingTicket.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("ForClosingTicket")]
        public async Task<IActionResult> ForClosingTicket([FromBody] ForClosingTicketCommand command)
        {
            try
            {
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


    }
}
