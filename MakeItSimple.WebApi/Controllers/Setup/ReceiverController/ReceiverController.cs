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
        public async Task<IActionResult> GetReceiverRole(GetReceiverRoleQuery query)
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
                var location = await _mediator.Send(query);

                Response.AddPaginationHeader(

                location.CurrentPage,
                location.PageSize,
                location.TotalCount,
                location.TotalPages,
                location.HasPreviousPage,
                location.HasNextPage

                );

                var result = new
                {
                    location,
                    location.CurrentPage,
                    location.PageSize,
                    location.TotalCount,
                    location.TotalPages,
                    location.HasPreviousPage,
                    location.HasNextPage
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
