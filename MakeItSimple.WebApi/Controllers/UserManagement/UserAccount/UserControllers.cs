using FluentValidation;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Feature.UserFeatures;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Feature.UserFeatures.GetUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.AddNewUser;
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

                var users = await _mediator.Send(query);

                Response.AddPaginationHeader(
                
                users.CurrentPage,
                users.PageSize,
                users.TotalCount,
                users.TotalPages,
                users.HasPreviousPage,
                users.HasNextPage
                
                );

                var result = new
                {
                    users,
                    users.CurrentPage,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages,
                    users.HasPreviousPage,
                    users.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
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

                if(User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.AddedBy = userId;
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

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.ModifiedBy = userId;
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


        [HttpPatch("UpdateUserStatus")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserCommand command)
        {
            try
            {

                var result = await _mediator.Send(command);
                if(result.IsFailure)
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











    }
}
