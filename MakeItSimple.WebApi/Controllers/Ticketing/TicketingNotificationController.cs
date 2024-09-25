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
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.GetTicketTransactionNotification;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.ClickedTransaction;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.GetAllTransactionNotification;

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

                var cacheKey = $"{userId}_{notificationType}_CacheKey";
                var timerKey = $"{userId}_{notificationType}";

                var newData = await _mediator.Send(command);
                var newHash = ComputeHash(newData);

                if (_cacheProvider.TryGetValue(cacheKey, out object cachedResult))
                {
                    var cachedHash = ComputeHash(cachedResult);

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
                        await _hubCaller.SendNotificationAsync(userId,notificationType,requestData);
                    }

                }, 5000, 5000);


                await _hubCaller.SendNotificationAsync(userId,notificationType,newData);

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

    [HttpGet("ticket-transaction")]
    public async Task<IActionResult> GetTicketTransactionNotification([FromQuery] GetTicketTransactionNotificationCommand command)
    {
        return await HandleNotification(command, "TransactionData");
    }

    [HttpGet("all-ticket-transaction")]
    public async Task<IActionResult> GetAllTransactionNotification([FromQuery] GetAllTransactionNotificationCommand command)
    {
        return await HandleNotification(command, "NotificationBellData");
    }


    [HttpPost("clicked-transaction")]
    public async Task<IActionResult> ClickedTransaction([FromBody] ClickedTransactionCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);

            if(result.IsFailure)
            {
                return BadRequest(result);
            }

            return Ok(result);  

        }
        catch(Exception ex)
        {
            return Conflict(ex.Message);
        }
    }


}
