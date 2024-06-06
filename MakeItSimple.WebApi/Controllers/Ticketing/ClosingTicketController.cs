using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.AddNewClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ApprovalClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetOpenTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.RejectClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.UpsertClosingTicket;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace MakeItSimple.WebApi.Controllers.Ticketing

{
    [Route("api/closing-ticket")]
    [ApiController]
    public class ClosingTicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClosingTicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewClosingTicket([FromBody] AddNewClosingTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Requestor_By = userId;
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

        [HttpPost("upsert/{id}")]
        public async Task<IActionResult> UpsertClosingTicket([FromBody] UpsertClosingTicketCommand command , [FromRoute] int id)
        {
            try
            {
                command.TicketTransactionId = id;
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.Modified_By = userId;
                        command.Added_By = userId;
                        command.Requestor_By = userId;
                    }
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);
            }
            catch(Exception ex)
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
                        command.Approver_By = userId;
                        command.Requestor_By = userId;
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
                        command.Approver_By = userId;
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
