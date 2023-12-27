
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup.AddNewTeam;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup.GetTeam;

namespace MakeItSimple.WebApi.Controllers.Setup.TeamController
{
    [Route("api/Team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;

        public TeamController(IMediator mediator , ValidatorHandler validatorHandler )
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;
        }

        [HttpPost("AddNewTeam")]
        public async Task<IActionResult> AddNewTeam([FromBody]AddNewTeamCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.AddNewTeamValidator.ValidateAsync(command);

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


        [HttpGet("GetTeam")]
        public async Task<IActionResult> GetTeam([FromQuery] GetTeamQuery query)
        {
            try
            {

                var team = await _mediator.Send(query);

                Response.AddPaginationHeader(

                team.CurrentPage,
                team.PageSize,
                team.TotalCount,
                team.TotalPages,
                team.HasPreviousPage,
                team.HasNextPage

                );

                var result = new
                {
                    team,
                    team.CurrentPage,
                    team.PageSize,
                    team.TotalCount,
                    team.TotalPages,
                    team.HasPreviousPage,
                    team.HasNextPage
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
