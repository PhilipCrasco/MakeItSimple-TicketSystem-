using FluentValidation;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Feature.UserFeatures.GetUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.UpdateUser;

namespace MakeItSimple.WebApi.Controllers.UserController
{
    [Route("api/User")]
    [ApiController]
    public class UserControllers : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;


        public UserControllers(IMediator mediator , ValidatorHandler validatorHandler)
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;

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



        [HttpPost("AddNewUser")]
        public async Task<IActionResult> AddNewUser([FromBody] AddNewUserCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.AddNewUserValidator.ValidateAsync(command);
               
                if(!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
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
                return BadRequest(ex.Message);
            }

        }


        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.UpdateUserValidator.ValidateAsync(command);
                if(!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);

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
                return BadRequest(ex.Message);
            }
        }









    }
}
