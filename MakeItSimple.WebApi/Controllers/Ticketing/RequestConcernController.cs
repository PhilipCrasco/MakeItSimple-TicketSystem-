using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddCommentNotificationValidator;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddDevelopingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddNewTicketAttachment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequestConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequestConcernReceiver;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddTicketComment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApproveRequestTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.CancelRequestConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.CancelTicketConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetRequestAttachment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetRequestConcern.GetRequestConcernResult;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetRequestorTicketConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetTicketComment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetTicketHistory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.RejectRequestTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.RemoveTicketAttachment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.RemoveTicketComment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.RequestApprovalReceiver;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ReturnRequestTicket;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/request-concern")]
    [ApiController]
    public class RequestConcernController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequestConcernController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("add-request-concern")]
        public async Task<IActionResult> AddRequestConcern([FromForm]AddRequestConcernCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                    command.Added_By = userId;
                    command.UserId = userId;
                   
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

        [HttpPut("cancel-request")]
        public async Task<IActionResult> CancelRequestConcern([FromBody] CancelRequestConcernCommand command)
        {
            try
            {
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


        [HttpPut("remove-attachment")]
        public async Task<IActionResult> RemoveTicketAttachment([FromBody] RemoveTicketAttachmentCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;

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


        [HttpPost("add-ticket-concern")]
        public async Task<IActionResult> AddRequestConcernReceiver([FromForm]AddRequestConcernReceiverCommand command)
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
                        command.Added_By = userId;
                        command.Modified_By = userId;
                        //command.IssueHandler = userId;

                    }
                }
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);


            }
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("add-development")]
        public async Task<IActionResult> AddDevelopingTicket([FromForm] AddDevelopingTicketCommand command)
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
                        command.Added_By = userId;
                        command.Modified_By = userId;
                        //command.IssueHandler = userId;

                    }
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

        [HttpPost("attachment/{id}")]
        public async Task<IActionResult> AddNewTicketAttachment([FromForm] AddNewTicketAttachmentCommand command, [FromRoute] int id)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Modified_By = userId;
                }

                command.RequestTransactionId = id;

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

        [HttpGet("request-attachment")]
        public async Task<IActionResult> GetRequestAttachment([FromQuery] GetRequestAttachmentQuery query)
        {
            try
            {
                var results = await _mediator.Send(query);
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

        [HttpPut("cancel-concern")]
        public async Task<IActionResult> CancelTicketConcern(CancelTicketConcernCommand command)
        {
            try
            {
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


        [HttpGet("requestor-page")]
        public async Task<IActionResult> GetRequestorTicketConcern([FromQuery] GetRequestorTicketConcernQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        query.UserId = userId;
                        
                    }
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }
                }

                var requestConcern = await _mediator.Send(query);

                Response.AddPaginationHeader(

                requestConcern.CurrentPage,
                requestConcern.PageSize,
                requestConcern.TotalCount,
                requestConcern.TotalPages,
                requestConcern.HasPreviousPage,
                requestConcern.HasNextPage

                );

                var result = new
                {
                    requestConcern,
                    requestConcern.CurrentPage,
                    requestConcern.PageSize,
                    requestConcern.TotalCount,
                    requestConcern.TotalPages,
                    requestConcern.HasPreviousPage,
                    requestConcern.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("page")]
        public async Task<IActionResult> GetRequestConcern([FromQuery] GetRequestConcernQuery query)
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
                        query.UserId = userId;

                    }
                }

                var requestConcern = await _mediator.Send(query);

                Response.AddPaginationHeader(

                requestConcern.CurrentPage,
                requestConcern.PageSize,
                requestConcern.TotalCount,
                requestConcern.TotalPages,
                requestConcern.HasPreviousPage,
                requestConcern.HasNextPage

                );

                var result = new
                {
                    requestConcern,
                    requestConcern.CurrentPage,
                    requestConcern.PageSize,
                    requestConcern.TotalCount,
                    requestConcern.TotalPages,
                    requestConcern.HasPreviousPage,
                    requestConcern.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        //[HttpPut("approve-request")]
        //public async Task<IActionResult> ApproveRequestTicket([FromBody] ApproveRequestTicketCommand command)
        //{
        //    try
        //    {
        //        if (User.Identity is ClaimsIdentity identity)
        //        {
        //            var userRole = identity.FindFirst(ClaimTypes.Role);
        //            if (userRole != null)
        //            {
        //                command.Role = userRole.Value;
        //            }

        //            if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
        //            {
        //                command.UserId = userId;
        //                command.Approved_By = userId;

        //            }
        //        }

        //        var results = await _mediator.Send(command);
        //        if (results.IsFailure)
        //        {

        //            return BadRequest(results);
        //        }
        //        return Ok(results);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Conflict(ex.Message);
        //    }
        //}

        [HttpPut("approval-request-receiver/{id}")]
        public async Task<IActionResult> RequestApprovalReceiver([FromBody] RequestApprovalReceiverCommand command ,[FromRoute] int id)
        {
            try
            {

                command.RequestTransactionId = id;
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.UserId = userId;
                        command.Approved_By = userId;

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

        [HttpPut("reject")]
        public async Task<IActionResult> RejectRequestTicket([FromBody] RejectRequestTicketCommand command)
        {
            try
            {


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


        [HttpPut("return")]
        public async Task<IActionResult> ReturnRequestTicket([FromBody] ReturnRequestTicketCommand command)
        {
            try
            {


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


        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetTicketHistory([FromRoute] int id)
        {
            try
            {
                var query = new GetTicketHistoryQuery
                {
                    TicketTransactionId = id
                };

                var results = await _mediator.Send(query);
                if (results.IsFailure)
                {
                    return BadRequest(query);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("add-comment")]
        public async Task<IActionResult> AddTicketComment([FromForm] AddTicketCommentCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                    command.Added_By = userId;
                    command.UserId = userId;

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

        [HttpPut("remove-comment")]
        public async Task<IActionResult> RemoveTicketComment([FromBody] RemoveTicketCommentCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;

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

        [HttpGet("view-comment")]
        public async Task<IActionResult> GetTicketComment([FromQuery]GetTicketCommentQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("add-comment-view")]
        public async Task<IActionResult> AddCommentNotificationValidator([FromBody] AddCommentNotificationValidatorCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;
                    command.Added_By = userId;  

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


    }
}
