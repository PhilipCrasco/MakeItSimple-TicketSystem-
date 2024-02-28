using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.RequestTicketNotification.RequestTicketNotificationQuery;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/ticketing-notification")]
    [ApiController]
    public class TicketingNotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketingNotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ticket-request")]
        public async Task<IActionResult> RequestTicketNotification([FromQuery] RequestTicketNotificationResultQuery command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;
                }
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
