using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.GetCompany;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.SyncCompany;

namespace MakeItSimple.WebApi.Controllers.Setup.CompanyController
{
    [Route("api/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly ValidatorHandler _validatorHandler;

        public CompanyController(IMediator mediator , ValidatorHandler validatorHandler)
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;
        }

        [HttpPost()]
        public async Task<IActionResult> SyncCompany([FromBody] SyncCompanyCommand command)
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
        public async Task<IActionResult> GetCompany([FromQuery] GetCompanyQuery query)
        {
            try
            {
                var company = await _mediator.Send(query);

                Response.AddPaginationHeader(

                company.CurrentPage,
                company.PageSize,
                company.TotalCount,
                company.TotalPages,
                company.HasPreviousPage,
                company.HasNextPage

                );

                var result = new
                {
                    company,
                    company.CurrentPage,
                    company.PageSize,
                    company.TotalCount,
                    company.TotalPages,
                    company.HasPreviousPage,
                    company.HasNextPage
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
