using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.DataAccessLayer.Feature.UserFeatures.GetUser;

namespace MakeItSimple.WebApi.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserControllers : ControllerBase
    {

        private readonly IMediator _mediator;

        public UserControllers(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromQuery] GetUsersQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
