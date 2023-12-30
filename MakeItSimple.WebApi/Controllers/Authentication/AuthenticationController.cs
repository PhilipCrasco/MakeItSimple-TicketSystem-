using MakeItSimple.WebApi.DataAccessLayer.Features.AuthenticationFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MakeItSimple.WebApi.Controllers.AuthenticationController
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IMediator _mediator;
        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("AuthenticateUser")]
        public async Task<ActionResult<AuthenticateUser.AuthenticateUserResult>> AuthenticateUser(AuthenticateUser.AuthenticateUserQuery request)
        {
            try
            {
                var result = await _mediator.Send(request);
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

    }
}
