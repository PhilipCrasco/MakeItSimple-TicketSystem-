using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate.AddTicketReDate;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate.GetReDate;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.ApprovedTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate.ApproveReDateTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.CancelTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate.CancelReDate;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.RejectTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate.RejectReDate;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/re-date")]
    [ApiController]
    public class ReDateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReDateController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> AddTicketReDate([FromForm] AddTicketReDateCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                    command.Added_By = userId;
                    command.Transacted_By = userId;

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
        public async Task<IActionResult> GetReDate([FromQuery] GetReDateQuery query)
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

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        //query.Users = userId;
                        query.UserId = userId;

                    }
                }

                var reDate = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reDate.CurrentPage,
                reDate.PageSize,
                reDate.TotalCount,
                reDate.TotalPages,
                reDate.HasPreviousPage,
                reDate.HasNextPage
                );

                var result = new
                {
                    reDate,
                    reDate.CurrentPage,
                    reDate.PageSize,
                    reDate.TotalCount,
                    reDate.TotalPages,
                    reDate.HasPreviousPage,
                    reDate.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("approval")]
        public async Task<IActionResult> ApproveReDateTicket([FromBody] ApproveReDateTicketCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.ReDate_By = userId;
                        command.Users = userId;
                        command.Transacted_By = userId;
                    }
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


        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelReDate([FromBody] CancelReDateCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Transacted_By = userId;
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


        [HttpPut("reject")]
        public async Task<IActionResult> RejectReDate([FromBody] RejectReDateCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.RejectReDate_By = userId;
                        command.Transacted_By = userId;
                    }
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

    }
}
