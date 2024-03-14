using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddDevelopingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequestConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequestConcernReceiver;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApproveRequestTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetRequestConcern.GetRequestConcernResult;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetTicketHistory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.RejectRequestTicket;


namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/request-concern")]
    [ApiController]
    public class RequestConcernController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequestConcernController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("add-request-concern")]
        public async Task<IActionResult> AddRequestConcern(AddRequestConcernCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                    command.Added_By = userId;
                    command.UserId = userId;
                   
                }
                var result = await _mediator.Send(command);
                if(result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("add-ticket-concern")]
        public async Task<IActionResult> AddRequestConcernReceiver(AddRequestConcernReceiverCommand command)
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
                        command.Added_By = userId;
                        command.Modified_By = userId;
                        command.IssueHandler = userId;

                    }
                }
                var result = await _mediator.Send(command);
                if (result.IsFailure)
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

        [HttpPost("add-development")]
        public async Task<IActionResult> AddDevelopingTicket([FromBody] AddDevelopingTicketCommand command)
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
                        command.Added_By = userId;
                        command.Modified_By = userId;
                        command.IssueHandler = userId;

                    }
                }
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);


            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }




        [HttpGet("page")]
        public async Task<IActionResult> GetRequestConcern([FromQuery] GetRequestConcernQuery query)
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

                var requestConcern = await _mediator.Send(query);

                Response.AddPaginationHeader(

                requestConcern.CurrentPage,
                requestConcern.PageSize,
                requestConcern.TotalCount,
                requestConcern.TotalPages,
                requestConcern.HasPreviousPage,
                requestConcern.HasNextPage

                );

                var result = new
                {
                    requestConcern,
                    requestConcern.CurrentPage,
                    requestConcern.PageSize,
                    requestConcern.TotalCount,
                    requestConcern.TotalPages,
                    requestConcern.HasPreviousPage,
                    requestConcern.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPut("approve-request")]
        public async Task<IActionResult> ApproveRequestTicket([FromBody] ApproveRequestTicketCommand command)
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
                        command.UserId = userId;
                        command.Approved_By = userId;

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
        public async Task<IActionResult> RejectRequestTicket([FromBody] RejectRequestTicketCommand command)
        {
            try
            {


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


        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetTicketHistory([FromRoute] int id)
        {
            try
            {
                var query = new GetTicketHistoryQuery
                {
                    TicketGeneratorId = id
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
