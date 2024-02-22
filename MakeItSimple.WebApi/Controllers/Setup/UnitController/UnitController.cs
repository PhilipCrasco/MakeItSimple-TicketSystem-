using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.SyncSubUnit;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.UnitSetup.SyncUnit;
using MakeItSimple.WebApi.Common.Extension;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup.GetSubUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.UnitSetup.GetUnit;
using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.Controllers.Setup.UnitController
{
    [Route("api/unit")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UnitController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> SyncUnit([FromBody] SyncUnitCommand command)
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
        public async Task<IActionResult> GetUnit([FromQuery] GetUnitQuery query)
        {
            try
            {

                var unit = await _mediator.Send(query);

                Response.AddPaginationHeader(

                unit.CurrentPage,
                unit.PageSize,
                unit.TotalCount,
                unit.TotalPages,
                unit.HasPreviousPage,
                unit.HasNextPage

                );

                var result = new
                {
                    unit,
                    unit.CurrentPage,
                    unit.PageSize,
                    unit.TotalCount,
                    unit.TotalPages,
                    unit.HasPreviousPage,
                    unit.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
