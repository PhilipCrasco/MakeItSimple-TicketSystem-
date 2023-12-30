﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.AddMember;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.AddNewChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.GetChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.RemoveChannelUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.UpdateChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.UpdateChannelStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.ChannelController
{
    [Route("api/Channel")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;
        
        public ChannelController(IMediator mediator , ValidatorHandler validatorHandler)
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;
        }

        [HttpPost("AddNewChannel")]
        public async Task<IActionResult> AddNewChannel([FromBody] AddNewChannelCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.AddNewChannelValidator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

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

        [HttpGet("GetChannel")]
        public async Task<IActionResult> GetChannel([FromQuery] GetChannelQuery query)
        {
            try
            {
                var channel = await _mediator.Send(query);

                Response.AddPaginationHeader(

                channel.CurrentPage,
                channel.PageSize,
                channel.TotalCount,
                channel.TotalPages,
                channel.HasPreviousPage,
                channel.HasNextPage

                );

                var result = new
                {
                    channel,
                    channel.CurrentPage,
                    channel.PageSize,
                    channel.TotalCount,
                    channel.TotalPages,
                    channel.HasPreviousPage,
                    channel.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UpdateChannel")]
        public async Task<IActionResult> UpdateChannel([FromBody] UpdateChannelCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.UpdateChannelValidator.ValidateAsync(command);

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
                return Conflict(ex.Message);
            }

        }

        [HttpPost("UpdateChannelStatus")]
        public async Task<IActionResult> UpdateChannelStatus([FromBody] UpdateChannelStatusCommand command)
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



        [HttpPost("AddMember")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberCommand command)
        {

            try
            {
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

        [HttpDelete("RemoveChannelUser")]
        public async Task<IActionResult> RemoveChannelUser([FromBody] RemoveChannelUserCommand command)
        {
            try
            {

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
                return Conflict(ex.Message);
            }

        }

    }
}
