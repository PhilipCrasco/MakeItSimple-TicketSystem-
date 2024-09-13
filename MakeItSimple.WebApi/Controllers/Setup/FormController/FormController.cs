using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup.GetDepartment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.AddNewForm;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.GetForm;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.UpdateSubUnitStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.UpdateFormStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.FormController
{
    [Route("api/form")]
    [ApiController]
    public class FormController : ControllerBase
    {

        private readonly IMediator _mediator;

        public FormController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewForm([FromBody] AddNewFormCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Modified_By = userId;
                }

                var result = await _mediator.Send(command);
                
                if(result.IsFailure)
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
        public async Task<IActionResult> GetForm([FromQuery] GetFormQuery query)
        {
            try
            {
                var form = await _mediator.Send(query);

                Response.AddPaginationHeader(

                form.CurrentPage,
                form.PageSize,
                form.TotalCount,
                form.TotalPages,
                form.HasPreviousPage,
                form.HasNextPage

                );

                var result = new
                {
                    form,
                    form.CurrentPage,
                    form.PageSize,
                    form.TotalCount,
                    form.TotalPages,
                    form.HasPreviousPage,
                    form.HasNextPage
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
        public async Task<IActionResult> UpdateFormStatus([FromRoute] int id)
        {
            try
            {
                var command = new UpdateFormStatusCommand
                {
                    Id = id
                };

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
