using MakeItSimple.WebApi.Common.SignalR;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.CommentNotification;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketingNotification;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/ticketing-notification")]
    [ApiController]
    public class TicketingNotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TimerControl _timerControl;
        private readonly IHubContext<NotificationHub> _hubContext;

        public TicketingNotificationController(IMediator mediator, IHubContext<NotificationHub> hubContext, TimerControl timerControl)
        {
            _mediator = mediator;
            _hubContext = hubContext;
            _timerControl = timerControl;
        }

        private async Task<IActionResult> HandleNotification<T>(T command, string notificationType)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        dynamic cmd = command;
                        cmd.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        dynamic cmd = command;
                        cmd.UserId = userId;
                    }
                }

                var results = await _mediator.Send(command);

                if (_timerControl != null && !_timerControl.IsTimerStarted)
                {
                    _timerControl.ScheduleTimer(async (scopeFactory) =>
                    {
                        using var scope = scopeFactory.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var requestData = await mediator.Send(command);
                        await _hubContext.Clients.All.SendAsync(notificationType, requestData);
                    }, 2000);
                }

                await _hubContext.Clients.All.SendAsync("ReceiveNotification", "New data has been received or sent.");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("ticket-notif")]
        public async Task<IActionResult> TicketingNotification([FromQuery] TicketingNotificationCommand command)
        {
            return await HandleNotification(command, "TicketNotifData");
        }

        //[HttpGet("transfer-ticket")]
        //public async Task<IActionResult> TransferTicketNotification([FromQuery] TransferTicketNotificationResultQuery command)
        //{
        //    return await HandleNotification(command, "TransferData");
        //}

        //[HttpGet("closing-ticket")]
        //public async Task<IActionResult> ClosingTicketNotification([FromQuery] ClosingTicketNotificationResultQuery command)
        //{
        //    return await HandleNotification(command, "ClosingData");
        //}

        //[HttpGet("open-ticket")]
        //public async Task<IActionResult> OpenTicketNotification([FromQuery] OpenTicketNotificationResultQuery command)
        //{
        //    return await HandleNotification(command, "OpenTicketData");
        //}

        [HttpGet("ticket-comment")]
        public async Task<IActionResult> CommentNotification([FromQuery] CommentNotificationQueryResult command)
        {
            return await HandleNotification(command, "CommentData");
        }
    }
}
