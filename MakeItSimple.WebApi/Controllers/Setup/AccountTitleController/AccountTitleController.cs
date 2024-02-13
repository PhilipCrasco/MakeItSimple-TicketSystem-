using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.AccountTitleSetup;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.AccountTitleSetup.GetAccountTitle;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.AccountTitleSetup.SyncAccountTitle;

namespace MakeItSimple.WebApi.Controllers.Setup.AccountTitleController
{
    [Route("api/account-title")]
    [ApiController]
    public class AccountTitleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountTitleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> SyncAccoutTitle([FromBody] SyncAccountTitleCommand command)
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
        public async Task<IActionResult> GetAccountTitle([FromQuery] GetAccountTitleQuery query)
        {
            try
            {
                var accountTitles = await _mediator.Send(query);

                Response.AddPaginationHeader(

                accountTitles.CurrentPage,
                accountTitles.PageSize,
                accountTitles.TotalCount,
                accountTitles.TotalPages,
                accountTitles.HasPreviousPage,
                accountTitles.HasNextPage

                );

                var result = new
                {
                    accountTitles,
                    accountTitles.CurrentPage,
                    accountTitles.PageSize,
                    accountTitles.TotalCount,
                    accountTitles.TotalPages,
                    accountTitles.HasPreviousPage,
                    accountTitles.HasNextPage
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
