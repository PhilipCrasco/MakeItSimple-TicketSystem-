using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.CreateOnHold.CreateOnHoldTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.ResumeOnHold.ResumeOnHoldTicket;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/on-hold")]
    [ApiController]
    public class OnHoldController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OnHoldController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOnHoldTicket([FromForm]CreateOnHoldTicketCommand command)
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


        [HttpPut("resume")]
        public async Task<IActionResult> ResumeOnHoldTicket([FromBody]ResumeOnHoldTicketCommand command)
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
