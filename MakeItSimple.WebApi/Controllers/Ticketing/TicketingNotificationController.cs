using MakeItSimple.WebApi.Common.SignalR;
using MakeItSimple.WebApi;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.CommentNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketingNotification;
using System.Security.Claims;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route("api/ticketing-notification")]
public class TicketingNotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubCaller _hubCaller;
    private readonly TimerControl _timerControl;
    private readonly IMemoryCache _memoryCache;

    public TicketingNotificationController(IMediator mediator, IHubCaller hubCaller, TimerControl timerControl , IMemoryCache memoryCache)
    {
        _mediator = mediator;
        _hubCaller = hubCaller;
        _timerControl = timerControl;
        _memoryCache = memoryCache; 
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

                //var timerKey = $"{userId}_{notificationType}";

                //_timerControl.ScheduleTimer(timerKey, async (scopeFactory) =>
                //{
                //    using var scope = scopeFactory.CreateScope();
                //    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                //    var requestData = await mediator.Send(command);
                //    await _hubCaller.SendNotificationAsync(userId, requestData);
                //}, 2000, 2000);

                //await _hubCaller.SendNotificationAsync(userId, result);

                //return Ok(result);

                //var result = await _mediator.Send(command);

                var timerKey = $"{userId}_{notificationType}";

                _timerControl.ScheduleTimer(timerKey, async (scopeFactory) =>
                {
                    using var scope = scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var requestData = await mediator.Send(command);
                    await _hubCaller.SendNotificationAsync(userId, requestData);
                }, 2000, 2000);

                await _hubCaller.SendNotificationAsync(userId, result);

                return Ok(result);

                //var cacheKey = $"{userId}_{notificationType}";
                //var previousResult = _memoryCache.Get<object>(cacheKey);

                //if (result != null && !Equals(result, previousResult))
                //{
                //    await _hubCaller.SendNotificationAsync(userId, result);
                //    _memoryCache.Set(cacheKey, result); // Update cache with new result
                //}

                //return Ok(result);


            }
            else
            {
                return Unauthorized("User identity is not valid.");
            }
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

    [HttpGet("ticket-comment")]
    public async Task<IActionResult> CommentNotification([FromQuery] CommentNotificationQueryResult command)
    {
        return await HandleNotification(command, "CommentData");
    }
}
