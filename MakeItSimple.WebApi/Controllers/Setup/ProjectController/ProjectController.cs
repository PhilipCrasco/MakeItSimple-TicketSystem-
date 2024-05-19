using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ProjectSetup.GetProject;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ProjectSetup.UpdateProjectStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ProjectSetup.UpsertProject;

namespace MakeItSimple.WebApi.Controllers.Setup.ProjectController
{
    [Route("api/project")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> UpsertProject([FromBody] UpsertProjectCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Modified_By = userId;
                }

                var results = await _mediator.Send(command);
                if(results.IsFailure)
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
        public async Task<IActionResult> GetProject([FromQuery] GetProjectQuery query )
        {
            try
            {
                var project = await _mediator.Send(query);

                Response.AddPaginationHeader(

                project.CurrentPage,
                project.PageSize,
                project.TotalCount,
                project.TotalPages,
                project.HasPreviousPage,
                project.HasNextPage

                );

                var result = new
                {
                    project,
                    project.CurrentPage,
                    project.PageSize,
                    project.TotalCount,
                    project.TotalPages,
                    project.HasPreviousPage,
                    project.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpsertProject([FromRoute] int id)
        {
            try
            {
                var command = new UpdateProjectStatusCommand
                {
                     Id  = id
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


    }
}
