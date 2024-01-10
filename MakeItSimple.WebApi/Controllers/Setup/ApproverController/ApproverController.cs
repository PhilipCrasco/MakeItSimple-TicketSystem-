using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup.AddNewApprover;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup.GetApprover;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup.UpdateApproverStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.ApproverController
{
    [Route("api/Approver")]
    [ApiController]
    public class ApproverController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApproverController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("AddNewApprover")]
        public async Task<IActionResult>AddNewApprover([FromBody] AddNewApproverCommand command)
        {

            try
            {

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

        [HttpGet("GetApprover")]
        public async Task<IActionResult> GetApprover([FromQuery] GetApproverQuery query)
        {

                var approver = await _mediator.Send(query);

                Response.AddPaginationHeader(

                approver.CurrentPage,
                approver.PageSize,
                approver.TotalCount,
                approver.TotalPages,
                approver.HasPreviousPage,
                approver.HasNextPage

                );

                var result = new
                {
                    approver,
                    approver.CurrentPage,
                    approver.PageSize,
                    approver.TotalCount,
                    approver.TotalPages,
                    approver.HasPreviousPage,
                    approver.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

        }

        [HttpPatch("UpdateApproverStatus")]
        public async Task<IActionResult> UpdateApproverStatus([FromBody] UpdateApproverStatusCommand command)
        {
            try
            {
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
