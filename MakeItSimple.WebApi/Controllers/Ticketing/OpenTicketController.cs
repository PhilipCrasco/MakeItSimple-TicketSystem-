using MakeItSimple.WebApi.Common.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetOpenTicket;
using System.Security.Claims;
using MediatR;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ReturnOpenTicket;
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

        [HttpPut("return/{id}")]
        public async Task<IActionResult> ReturnOpenTicket([FromBody]ReturnOpenTicketCommand command , [FromRoute] int id)
        {
            try
            {
                command.RequestTransactionId = id;

                var result = await _mediator.Send(command); 
                if(result.IsFailure) 
                {
                    return BadRequest(result);
                }
                return Ok(result);
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
