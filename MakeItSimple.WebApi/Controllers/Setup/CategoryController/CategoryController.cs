using FluentValidation;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup.GetCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup.UpdateCategoryStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup.UpsertCategory;

namespace MakeItSimple.WebApi.Controllers.Setup.CategoryController
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;

        public CategoryController(IMediator mediator , ValidatorHandler validatorHandler)
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;
        }

        [HttpPost]
        public async Task<IActionResult> UpsertCategory([FromBody] UpsertCategoryCommand command)
        {

            try
            {
                var validationResult = await _validatorHandler.UpsertCategoryValidator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

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
        public async Task<IActionResult> GetCategory([FromQuery] GetCategoryQuery  query )
        {
            try
            {
                var category = await _mediator.Send(query);

                Response.AddPaginationHeader(

                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasPreviousPage,
                category.HasNextPage

                );

                var result = new
                {
                    category,
                    category.CurrentPage,
                    category.PageSize,
                    category.TotalCount,
                    category.TotalPages,
                    category.HasPreviousPage,
                    category.HasNextPage
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
        public async Task<IActionResult> UpdateCategoryStatus([FromRoute] int id)
        {
            try
            {
                var command = new UpdateCategoryStatusCommand
                {
                    Id = id
                };
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

    }
}
