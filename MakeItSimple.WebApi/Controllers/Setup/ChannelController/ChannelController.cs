

using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.AddNewChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.GetChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.GetChannelValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.GetMember;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.UpdateChannelStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.ChannelController
{
    [Route("api/channel")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;

        public ChannelController(IMediator mediator, ValidatorHandler validatorHandler)
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;
        }


        [HttpGet("page")]
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

        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpdateChannelStatus([FromRoute] int id)
        {

            try
            {
                var command = new UpdateChannelStatusCommand
                {
                    Id = id
                };
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



        [HttpGet("member-list")]
        public async Task<IActionResult> GetMember([FromQuery]GetMemberQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return Conflict(ex.Message);
            }
        }


        [HttpPost("validation")]
        public async Task<IActionResult> GetChannelValidation(GetChannelValidationCommand command)
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

        [HttpPost]
        public async Task<ActionResult> AddNewChannels([FromBody] AddNewChannelCommands command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
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
