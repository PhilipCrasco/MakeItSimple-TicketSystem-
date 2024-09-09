using MakeItSimple.WebApi;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.CommentNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketingNotification;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using MakeItSimple.WebApi.Common.SignalR;
using LazyCache;
using MakeItSimple.WebApi.Common.Caching;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;

[ApiController]
[Route("api/ticketing-notification")]
public class TicketingNotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubCaller _hubCaller;
    private readonly TimerControl _timerControl;

    private ICacheProvider _cacheProvider;

    public TicketingNotificationController(IMediator mediator, IHubCaller hubCaller, TimerControl timerControl , ICacheProvider cacheProvider)
    {
        _mediator = mediator;
        _hubCaller = hubCaller;
        _timerControl = timerControl;
        _cacheProvider = cacheProvider;
    }

    private string ComputeHash(object obj)
    {
        var jsonString = JsonConvert.SerializeObject(obj);
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(jsonString));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    //private async Task<IActionResult> HandleNotification<T>(T command, string notificationType)
    //{
    //    try
    //    {

    //        if (User.Identity is ClaimsIdentity identity &&
    //            Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
    //        {
    //            dynamic cmd = command;
    //            cmd.UserId = userId;
    //            cmd.Role = identity.FindFirst(ClaimTypes.Role)?.Value;


    //            var newData = await _mediator.Send(command);

    //            var timerKey = $"{userId}_{notificationType}";

    //            if (_cacheProvider.TryGetValue(CacheKeys.TicketingNotif, out object cachedResult))
    //            {

    //                var cachedHash = ComputeHash(cachedResult);
    //                var newHash = ComputeHash(newData);

    //                if (cachedHash == newHash)
    //                {

    //                    //var timerKey = $"{userId}_{notificationType}";
    //                    _timerControl.StopTimer(timerKey);

    //                    return Ok(cachedResult);
    //                }
    //            }

    //            var cacheEntryOptions = new MemoryCacheEntryOptions
    //            {
    //                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
    //                SlidingExpiration = TimeSpan.FromDays(1),
    //                Size = 1024
    //            };

    //            _cacheProvider.Set(CacheKeys.TicketingNotif, newData, cacheEntryOptions);


    //            _timerControl.ScheduleTimer(timerKey, async (scopeFactory) =>
    //            {
    //                using var scope = scopeFactory.CreateScope();
    //                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    //                var requestData = await mediator.Send(command);


    //                var requestDataHash = ComputeHash(requestData);
    //                var lastDataHash = ComputeHash(_cacheProvider.Get(CacheKeys.TicketingNotif));

    //                if (requestDataHash != lastDataHash)
    //                {
    //                    await _hubCaller.SendNotificationAsync(userId, requestData);
    //                }


    //            }, 2000, 2000);

    //            await _hubCaller.SendNotificationAsync(userId, newData);

    //            return Ok(newData);
    //        } 
    //        else
    //        {
    //            return Unauthorized("User not autorized");
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        return Conflict(ex.Message);
    //    }
    //}

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

                var newData = await _mediator.Send(command);


                var cacheKey = $"{userId}_{notificationType}_CacheKey";


                var timerKey = $"{userId}_{notificationType}";


                if (_cacheProvider.TryGetValue(cacheKey, out object cachedResult))
                {
                    var cachedHash = ComputeHash(cachedResult);
                    var newHash = ComputeHash(newData);


                    if (cachedHash == newHash)
                    {
                        _timerControl.StopTimer(timerKey); 
                        return Ok(cachedResult);
                    }
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                    SlidingExpiration = TimeSpan.FromDays(1),
                    Size = 1024
                };

                _cacheProvider.Set(cacheKey, newData, cacheEntryOptions);

                _timerControl.ScheduleTimer(timerKey, async (scopeFactory) =>
                {
                    using var scope = scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var requestData = await mediator.Send(command);

                    var requestDataHash = ComputeHash(requestData);
                    var lastDataHash = ComputeHash(_cacheProvider.Get(cacheKey));

                    if (requestDataHash != lastDataHash)
                    {

                        _cacheProvider.Set(cacheKey, requestData, cacheEntryOptions);
                        await _hubCaller.SendNotificationAsync(userId, requestData);
                    }

                }, 2000, 2000);


                await _hubCaller.SendNotificationAsync(userId, newData);

                return Ok(newData);
            }
            else
            {
                return Unauthorized("User not authorized");
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
