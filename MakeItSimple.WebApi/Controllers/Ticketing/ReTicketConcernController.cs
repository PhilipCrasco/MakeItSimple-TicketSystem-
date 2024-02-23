using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.AddNewReTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.ApproveReTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.CancelReTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.GetReTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.RejectReTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.UpsertReTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.UpsertReTicketAttachment;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/re-ticket")]
    [ApiController]
    public class ReTicketConcernController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReTicketConcernController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewReTicket([FromBody] AddNewReTicketCommand command)
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

        [HttpGet("page")]
        public async Task<IActionResult> GetReTicket([FromQuery] GetReTicketQuery query)
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
                        //query.Users = userId;
                        query.UserId = userId;
                    }
                }
                var ticketRequest = await _mediator.Send(query);

                Response.AddPaginationHeader(

                ticketRequest.CurrentPage,
                ticketRequest.PageSize,
                ticketRequest.TotalCount,
                ticketRequest.TotalPages,
                ticketRequest.HasPreviousPage,
                ticketRequest.HasNextPage

                );

                var result = new
                {
                    ticketRequest,
                    ticketRequest.CurrentPage,
                    ticketRequest.PageSize,
                    ticketRequest.TotalCount,
                    ticketRequest.TotalPages,
                    ticketRequest.HasPreviousPage,
                    ticketRequest.HasNextPage
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
        public async Task<IActionResult> ApproveReTicket([FromBody] ApproveReTicketCommand command)
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
                        command.Re_Ticket_By = userId;
                        command.Users = userId;
                        command.Approver_By = userId;
                        command.Requestor_By = userId;
                    }
                }
                var results = await _mediator.Send(command);
                if(results.IsFailure)
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

        [HttpPost("upsert/{id}")]
        public async Task<IActionResult> UpsertReTicket([FromBody] UpsertReTicketCommand command ,[FromRoute] int id)
        {
            try
            {
                command.RequestGeneratorId = id;
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

        [HttpPost("attachment/{id}")]
        public async Task<IActionResult> UpsertReTicketAttachment([FromForm] UpsertReTicketAttachmentCommand command, [FromRoute] int id)
        {
            try
            {
                command.RequestGeneratorId = id;
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
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

        [HttpPut("reject")]
        public async Task<IActionResult> RejectReTicket([FromBody] RejectReTicketCommand command)
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
                        command.RejectReTicket_By = userId;
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

        [HttpPut("cancel")]
        public async Task<IActionResult> CancelReTicket([FromBody]CancelReTicketCommand command)
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

    }
}
