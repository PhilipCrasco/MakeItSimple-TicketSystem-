using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.AddNewTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.AddNewTicketAttachment;

namespace MakeItSimple.WebApi.Controllers.Ticketing.TicketRequest
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
        public async Task<IActionResult> AddNewTicket([FromBody] AddNewTicketCommand command )
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                              
                }

                var results = await _mediator.Send(command);
                if(results.IsFailure)
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
        public async Task<IActionResult> AddNewTicketAttachment([FromForm] AddNewTicketAttachmentCommand command , [FromRoute] int id)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                }

                command.TicketTransactionId = id;
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
