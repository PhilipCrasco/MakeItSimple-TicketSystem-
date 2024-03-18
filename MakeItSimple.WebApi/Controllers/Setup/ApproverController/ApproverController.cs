using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup.AddNewApprover;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup.GetApprover;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup.GetApproverRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ApproverSetup.UpdateApproverStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.GetMember;

namespace MakeItSimple.WebApi.Controllers.Setup.ApproverController
{
    [Route("api/approver")]
    [ApiController]
    public class ApproverController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApproverController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult>AddNewApprover([FromBody] AddNewApproverCommand command , [FromRoute] int id)
        {

            try
            {
                command.ChannelId = id; 
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

        [HttpGet("approver-user")]
        public async Task<IActionResult> GetApproverRole([FromQuery] GetApproverRoleQuery query)       
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }

                }
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return Conflict(ex.Message);
            }
        }

        [HttpPatch("status")]
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
