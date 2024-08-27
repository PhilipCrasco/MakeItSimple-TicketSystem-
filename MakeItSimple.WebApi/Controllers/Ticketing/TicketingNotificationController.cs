using MakeItSimple.WebApi.Common.SignalR;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.CommentNotification;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketingNotification;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/ticketing-notification")]
    [ApiController]
    public class TicketingNotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TimerControl _timerControl;
        private readonly IHubContext<NotificationHub> _client;

        public TicketingNotificationController(IMediator mediator, IHubContext<NotificationHub> client, TimerControl timerControl)
        {
            _mediator = mediator;
            _client = client;
            _timerControl = timerControl;
        }

        private async Task<IActionResult> HandleNotification<T>(T command, string notificationType)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity &&
                    Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    dynamic cmd = command;
                    cmd.UserId = userId;
                    cmd.Role = identity.FindFirst(ClaimTypes.Role)?.Value;

                    var result = await _mediator.Send(command);

                    if (_timerControl != null && !_timerControl.IsTimerStarted)
                    {
                        _timerControl.ScheduleTimer(async (scopeFactory) =>
                        {
                            using var scope = scopeFactory.CreateScope();
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            var requestData = await mediator.Send(command);
                            await _client.Clients.Group(userId.ToString()).SendAsync(notificationType, requestData);
                        }, 2000);
                    }

                    await _client.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", "New data has been received or sent.");

                    return Ok(result);
                }

                return BadRequest("User ID not found.");
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Message))
            {
                return BadRequest("Invalid notification data.");
            }

            var userId = User.FindFirstValue("id");

            if (Guid.TryParse(userId, out var guidUserId))
            {
                await _client.Clients.Group(guidUserId.ToString()).SendAsync("ReceiveNotification", model.Message);
                return Ok("Notification sent.");
            }

            return BadRequest("User ID not found.");
        }

        [HttpGet("ticket-notif")]
        public async Task<IActionResult> TicketingNotification([FromQuery] TicketingNotificationCommand command)
        {
            return await HandleNotification(command, "TicketNotifData");
        }


        [HttpGet("ticket-comment")]
        public async Task<IActionResult> CommentNotification([FromQuery] CommentNotificationQueryResult command)
        {
            return await HandleNotification(command, "CommentData");
        }

        public class SendNotificationModel
        {
            public Guid UserId { get; set; }
            public string Message { get; set; }
        }
    }
}
