using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup.GetTeam;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup.UpdateTeamStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup.UpsertTeam;

namespace MakeItSimple.WebApi.Controllers.Setup.TeamController
{
    [Route("api/team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> UpsertTeam([FromBody]UpsertTeamCommand command)
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
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpdateTeamStatus([FromRoute] int id)
        {
            try
            {
                var command = new UpdateTeamStatusCommand
                {
                    Id = id
                };

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
    }
}
