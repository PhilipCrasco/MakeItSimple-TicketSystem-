using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common.SignalR;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.AddNewClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ApprovalClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.CancelClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ConfirmClosedTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.RejectClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ReturnClosedTicket;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace MakeItSimple.WebApi.Controllers.Ticketing

{
    [Route("api/closing-ticket")]
    [ApiController]
    public class ClosingTicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TimerControl _timerControl;
        private readonly IHubContext<NotificationHub> _client;

        public ClosingTicketController(IMediator mediator, TimerControl timerControl, IHubContext<NotificationHub> client)
        {
            _mediator = mediator;
            _timerControl = timerControl;
            _client = client;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewClosingTicket([FromForm] AddNewClosingTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Modified_By = userId;
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


        [HttpGet("page")]
        public async Task<IActionResult> GetClosingTicket([FromQuery] GetClosingTicketQuery query)
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

                //var timerControl = _timerControl;
                //var clientsAll = _client.Clients.All;

                //if (timerControl != null && !timerControl.IsTimerStarted && clientsAll != null)
                //{
                //    timerControl.ScheduleTimer(async (scopeFactory) =>
                //    {
                //        using var scope = scopeFactory.CreateScope();
                //        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                //        var requestData = await mediator.Send(query);
                //        await clientsAll.SendAsync("TicketData", requestData);
                //    }, 2000);
                //}

                //await _client.Clients.All.SendAsync("ReceiveNotification", "New data has been received or sent.");

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpPut("approval")]
        public async Task<IActionResult> ApprovalClosingTicket([FromBody] ApproveClosingTicketCommand command)
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
                        command.Closed_By = userId;
                        command.Users = userId;
                        command.Transacted_By = userId;
                    }
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

        [HttpPut("reject")]
        public async Task<IActionResult> RejectClosingTicket([FromBody] RejectClosingTicketCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.RejectClosed_By = userId;
                        command.Transacted_By = userId;
                    }
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

        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelClosingTicket([FromBody] CancelClosingTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command. Transacted_By = userId;
                    }
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

        [HttpPut("confirmation")]
        public async Task<IActionResult> ConfirmClosedTicket([FromBody] ConfirmClosedTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.Transacted_By = userId;
                    }
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

        [HttpPut("return")]
        public async Task<IActionResult> ReturnClosedTicket([FromForm] ReturnClosedTicketCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.Added_By = userId;
                    }
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
