using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.BusinessUnitSetup.SyncBusinessUnit;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup.GetReceiverRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup.AddNewReceiver;
using MakeItSimple.WebApi.Common.Extension;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.LocationSetup.GetLocation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup.GetReceiver;
using MakeItSimple.WebApi.Common;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup.GetReceiverBusinessUnit;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup.UpdateApproverStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup.UpdateReceiverStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.UpdateChannelStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.ReceiverController
{
    [Route("api/receiver")]
    [ApiController]
    public class ReceiverController : ControllerBase
    {

        private readonly IMediator _mediator;

        public ReceiverController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("receiver-list")]
        public async Task<IActionResult> GetReceiverRole([FromQuery]GetReceiverRoleQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch(Exception ex) 
            {

                return Conflict(ex.Message);
            }
        }

        [HttpGet("receiver-business-unit")]
        public async Task<IActionResult> GetReceiverBusinessUnit([FromQuery]GetReceiverBusinessUnitQuery query)
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

        [HttpPut("status/{UserId}")]
        public async Task<IActionResult> UpdateApproverStatus([FromRoute] Guid?  UserId)
        {
            try
            {

                var command = new UpdateReceiverStatusCommand
                {
                    UserId = UserId
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return Conflict(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewReceiver([FromBody] AddNewReceiverCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Modified_By = userId;
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


        [HttpGet("page")]
        public async Task<IActionResult> GetReceiver([FromQuery] GetReceiverQuery query)
        {
            try
            {
                var receiver = await _mediator.Send(query);

                Response.AddPaginationHeader(

                receiver.CurrentPage,
                receiver.PageSize,
                receiver.TotalCount,
                receiver.TotalPages,
                receiver.HasPreviousPage,
                receiver.HasNextPage

                );

                var result = new
                {
                    receiver,
                    receiver.CurrentPage,
                    receiver.PageSize,
                    receiver.TotalCount,
                    receiver.TotalPages,
                    receiver.HasPreviousPage,

                    receiver.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
}
