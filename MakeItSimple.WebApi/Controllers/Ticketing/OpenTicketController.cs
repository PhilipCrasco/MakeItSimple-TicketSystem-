using MakeItSimple.WebApi.Common.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetOpenTicket;
using System.Security.Claims;
using MediatR;
using MakeItSimple.WebApi.Common;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetTicketHistory;
using MakeItSimple.WebApi.Common.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/open-ticket")]
    [ApiController]
    public class OpenTicketController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly TimerControl _timerControl;
        private readonly IHubContext<NotificationHub> _client;

        public OpenTicketController(IMediator mediator, TimerControl timerControl, IHubContext<NotificationHub> client)
        {
            _mediator = mediator;
            _timerControl = timerControl;
            _client = client;
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

                var timerControl = _timerControl;
                var clientsAll = _client.Clients.All;

                if (timerControl != null && !timerControl.IsTimerStarted && clientsAll != null)
                {
                    timerControl.ScheduleTimer(async (scopeFactory) =>
                    {
                        using var scope = scopeFactory.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var requestData = await mediator.Send(query);
                        await clientsAll.SendAsync("TicketData", requestData);
                    }, 2000);
                }

                await _client.Clients.All.SendAsync("ReceiveNotification", "New data has been received or sent.");

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
                    TicketConcernId = id
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
