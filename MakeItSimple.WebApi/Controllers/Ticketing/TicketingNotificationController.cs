
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.ClosingTicketNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.CommentNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.OpenTicketNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketConcernNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.ReTicketNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TransferTicketNotification;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


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


        [HttpGet("closing-ticket")]
        public async Task<IActionResult> ClosingTicketNotification([FromQuery] ClosingTicketNotificationResultQuery command)
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


        [HttpGet("open-ticket")]
        public async Task<IActionResult> OpenTicketNotification([FromQuery] OpenTicketNotificationResultQuery command)
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

        [HttpGet("ticket-comment")]
        public async Task<IActionResult> CommentNotification([FromQuery] CommentNotificationQueryResult command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        //query.Users = userId;
                        command.UserId = userId;

                    }
                }

                var results = await _mediator.Send(command);
                await _hubContext.Clients.All.SendAsync("SendingMessage", results);

                return Ok(results);

            }
            catch(Exception ex) 
            {
                return Conflict(ex.Message);

            }
        }




    }
}
