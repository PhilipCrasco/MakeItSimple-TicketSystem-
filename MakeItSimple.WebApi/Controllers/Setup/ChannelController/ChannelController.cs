using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.AddNewChannel;

namespace MakeItSimple.WebApi.Controllers.Setup.ChannelController
{
    [Route("api/Channel")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public ChannelController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("AddNewChannel")]
        public async Task<IActionResult> AddNewChannel([FromBody] AddNewChannelCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
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
    }
}
