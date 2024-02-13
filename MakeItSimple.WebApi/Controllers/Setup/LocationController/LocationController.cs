using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.LocationSetup;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.LocationSetup.GetLocation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.LocationSetup.SyncLocation;

namespace MakeItSimple.WebApi.Controllers.Setup.LocationController
{
    [Route("api/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> SyncLocation([FromBody] SyncLocationCommand command)
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

        [HttpGet("page")]
        public async Task<IActionResult> GetLocation([FromQuery] GetLocationQuery query)
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
