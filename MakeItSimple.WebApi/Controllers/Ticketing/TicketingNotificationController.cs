using MakeItSimple.WebApi.DataAccessLayer.Features.SignalRNotification;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.RequestTicketNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.ReTicketNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TransferTicketNotification;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/ticketing-notification")]
    [ApiController]
    public class TicketingNotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<NotificationHub> _hubContext;

        public TicketingNotificationController(IMediator mediator , IHubContext<NotificationHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
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

                // Notify clients using SignalR
                await _hubContext.Clients.All.SendAsync("SendingMessage", results);

                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("transfer-ticket")]
        public async Task<IActionResult> TransferTicketNotification([FromQuery] TransferTicketNotificationResultQuery command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        //query.Users = userId;
                        command.UserId = userId;

                    }
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }

                // Notify clients using SignalR
                await _hubContext.Clients.All.SendAsync("SendingMessage", results);

                return Ok(results);
            }
            catch (Exception ex)              
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("re-ticket")]
        public async Task<IActionResult> ReTicketNotification([FromQuery] ReTicketNotificationResultQuery command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        //query.Users = userId;
                        command.UserId = userId;

                    }
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }

                // Notify clients using SignalR
                await _hubContext.Clients.All.SendAsync("SendingMessage", results);

                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


    }
}
