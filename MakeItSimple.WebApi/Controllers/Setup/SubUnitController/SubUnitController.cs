
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.AddNewSubUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.UpdateSubUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.UpdateSubUnitStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup.GetSubUnit;

namespace MakeItSimple.WebApi.Controllers.Setup.TeamController
{
    [Route("api/SubUnit")]
    [ApiController]
    public class SubUnitController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;

        public SubUnitController(IMediator mediator , ValidatorHandler validatorHandler )
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;
        }

        [HttpPost("AddNewSubUnit")]
        public async Task<IActionResult> AddNewTeam([FromBody] AddNewSubUnitCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.AddNewSubUnitValidator.ValidateAsync(command);

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


        [HttpGet("GetSubUnit")]
        public async Task<IActionResult> GetSubUnit([FromQuery] GetSubUnitQuery query)
        {
            try
            {

                var subUnit = await _mediator.Send(query);

                Response.AddPaginationHeader(

                subUnit.CurrentPage,
                subUnit.PageSize,
                subUnit.TotalCount,
                subUnit.TotalPages,
                subUnit.HasPreviousPage,
                subUnit.HasNextPage

                );

                var result = new
                {
                    subUnit,
                    subUnit.CurrentPage,
                    subUnit.PageSize,
                    subUnit.TotalCount,
                    subUnit.TotalPages,
                    subUnit.HasPreviousPage,
                    subUnit.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("UpdateSubUnit")]
        public async Task<IActionResult> UpdateSubUnit([FromBody] UpdateSubUnitCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.UpdateSubUnitValidator.ValidateAsync(command);

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


        [HttpPost("UpdateSubUnitStatus")]
        public async Task<IActionResult> UpdateSubUnitStatus([FromBody] UpdateSubUnitStatusCommand command)
        {
            try
            {

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
