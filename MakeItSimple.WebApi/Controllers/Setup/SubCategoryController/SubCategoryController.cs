using FluentValidation;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup.GetSubCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup.UpdateSubCategoryStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup.UpsertSubCategory;

namespace MakeItSimple.WebApi.Controllers.Setup.SubCategoryController
{
    [Route("api/sub-category")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;

        public SubCategoryController(IMediator mediator , ValidatorHandler validatorHandler)
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler ;
        }

        [HttpPost]
        public async Task<IActionResult> UpsertSubCategory([FromBody] UpsertSubCategoryCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.UpsertSubCategoryValidator.ValidateAsync(command);

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
        public async Task<IActionResult> GetSubCategory([FromQuery]GetSubCategoryQuery query)
        {
            try
            {
                var subCategory = await _mediator.Send(query);

                Response.AddPaginationHeader(

                subCategory.CurrentPage,
                subCategory.PageSize,
                subCategory.TotalCount,
                subCategory.TotalPages,
                subCategory.HasPreviousPage,
                subCategory.HasNextPage

                );

                var result = new
                {
                    subCategory,
                    subCategory.CurrentPage,
                    subCategory.PageSize,
                    subCategory.TotalCount,
                    subCategory.TotalPages,
                    subCategory.HasPreviousPage,
                    subCategory.HasNextPage
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
        public async Task<IActionResult> UpdateSubCategoryStatus([FromRoute] int id)
        {
            try
            {
                var command = new UpdateSubCategoryStatusCommand
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
