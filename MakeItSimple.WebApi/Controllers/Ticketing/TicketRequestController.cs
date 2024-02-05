using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.AddNewTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.AddNewTicketAttachment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.ApproveRequestTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.CancelTicketRequest;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.GetRequestAttachment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.GetTicketRequest;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.RejectRequestTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.UpdateTicketRequest;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/TicketRequest")]
    [ApiController]
    public class TicketRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("AddNewTicket")]
        public async Task<IActionResult> AddNewTicket([FromBody] AddNewTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;

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

        [HttpPost("AddNewTicketAttachment/{id}")]
        public async Task<IActionResult> AddNewTicketAttachment([FromForm] AddNewTicketAttachmentCommand command, [FromRoute] int id)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Modified_By = userId;
                }

                command.RequestGeneratorId = id;

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

        [HttpGet("GetTicketRequest")]
        public async Task<IActionResult> GetTicketRequest([FromQuery] GetTicketRequestQuery query)
        {
            try
            {
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

        [HttpGet("GetRequestAttachment")]
        public async Task<IActionResult> GetRequestAttachment([FromQuery] GetRequestAttachmentQuery query)
        {
            try
            {
                var results = await _mediator.Send(query);
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

        [HttpPut("UpdateTicketRequest")]
        public async Task<IActionResult> UpdateTicketRequest([FromBody] UpdateTicketRequestCommand command)
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

        [HttpPut("ApproveRequestTicket")]
        public async Task<IActionResult> ApproveRequestTicket([FromBody] ApproveRequestTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Approved_By = userId;

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

        [HttpPut("RejectRequestTicket")]
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

        [HttpPut("CancelTicketRequest")]
        public async Task<IActionResult> CancelTicketRequest(CancelTicketRequestCommand command)
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
