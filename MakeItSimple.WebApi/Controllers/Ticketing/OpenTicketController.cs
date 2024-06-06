using MakeItSimple.WebApi.Common.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetOpenTicket;
using System.Security.Claims;
using MediatR;
using MakeItSimple.WebApi.Common;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetTicketHistory;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/open-Ticket")]
    [ApiController]
    public class OpenTicketController : ControllerBase
    {

        private readonly IMediator _mediator;

        public OpenTicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetOpenTicket([FromQuery] GetOpenTicketQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        query.UserId = userId;
                    }
                }
                var openTicket = await _mediator.Send(query);

                Response.AddPaginationHeader(

                openTicket.CurrentPage,
                openTicket.PageSize,
                openTicket.TotalCount,
                openTicket.TotalPages,
                openTicket.HasPreviousPage,
                openTicket.HasNextPage

                );

                var result = new
                {
                    openTicket,
                    openTicket.CurrentPage,
                    openTicket.PageSize,
                    openTicket.TotalCount,
                    openTicket.TotalPages,
                    openTicket.HasPreviousPage,
                    openTicket.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetTicketHistory([FromRoute] int id)
        {
            try
            {
                var query = new GetTicketHistoryQuery
                {
                    TicketTransactionId = id
                };

                var results = await _mediator.Send(query);
                if (results.IsFailure)
                {
                    return BadRequest(query);
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
