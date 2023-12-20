using FluentValidation;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.AddNewUserRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.GetUserRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.UntagAndTagUserRolePermission;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.UpdateUserRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.UpdateUserRoleStatus;

namespace MakeItSimple.WebApi.Controllers.UserManagement.UserRoleAccount
{
    [Route("api/UserRole")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;

        public UserRoleController(IMediator mediator , ValidatorHandler validatorHandler)
        {
            _validatorHandler = validatorHandler;
            _mediator = mediator;
        }

        [HttpPost("AddNewUserRole")]
        public async Task<IActionResult> AddNewUserRole([FromBody] AddNewUserRoleCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.AddUserRoleValidator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                }
                
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


        [HttpGet("GetUserRole")]
        public async Task<IActionResult> GetUserRole([FromQuery] GetUserRoleQuery query)
        {
            try
            {
                var userRole = await _mediator.Send(query);

                Response.AddPaginationHeader(

                userRole.CurrentPage,
                userRole.PageSize,
                userRole.TotalCount,
                userRole.TotalPages,
                userRole.HasPreviousPage,
                userRole.HasNextPage

                );

                var result = new
                {
                    userRole,
                    userRole.CurrentPage,
                    userRole.PageSize,
                    userRole.TotalCount,
                    userRole.TotalPages,
                    userRole.HasPreviousPage,
                    userRole.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPut("UpdateUserRole")]
        public async Task<IActionResult> UpdateUserResult([FromBody] UpdateUserRoleCommand command )
        {
            try
            {
                var validationResult = await _validatorHandler.UpdateUserRoleValidator.ValidateAsync(command);
                if(!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                }

                var result = await _mediator.Send(command);
                if(result.IsFailure)
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


        [HttpPatch("UpdateUserRoleStatus")]
        public async Task<IActionResult> UpdateUserRoleStatus([FromBody] UpdateUserRoleStatusCommand command )
        {
            try
            {
                var result = await _mediator.Send(command);
                if(result.IsFailure )
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch( Exception ex ) 
            {
             return BadRequest(ex.Message);
            }
        }

        [HttpPut("UntagAndTagUserRolePermission")]
        public async Task<IActionResult> UntagAndTagUserRolePermission([FromBody] UntagAndTagUserRolePermissionCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.TagAndUntagUserRoleValidator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
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




    }
}
