using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.ApprovedTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.CancelTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.RejectTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.AddNewTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.UpsertTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.UpsertTransferAttachment;
using MakeItSimple.WebApi.Models;
using MoreLinq.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetTicketHistory;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/TransferTicket")]
    [ApiController]
    public class TransferTicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransferTicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("AddNewTransferTicket")]
        public async Task<IActionResult> AddNewTransferTicket([FromBody] AddNewTransferTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Transfer_By = userId;
                    command.Requestor_By = userId;
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

        [HttpPost("UpsertTransferAttachment/{id}")]
        public async Task<IActionResult> UpsertTransferAttachment([FromForm] UpserTransferAttachmentCommand command , [FromRoute] int id)
        {
            try
            {

                command.RequestGeneratorId = id;
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Modified_By = userId;
                    command.Requestor_By= userId;
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
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


        [HttpPost("UpsertTransferTicket")]
        public async Task<IActionResult> UpsertTransferTicket([FromBody] UpsertTransferTicketCommand command)
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
                        command.Transfer_By = userId;
                        command.Requestor_By = userId;
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



        [HttpDelete("CancelTransferTicket")]
        public async Task<IActionResult> CancelTransferTicket([FromBody] CancelTransferTicketCommand command)
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


        [HttpGet("GetTransferTicket")]
        public async Task<IActionResult> GetTransferTicket([FromQuery] GetTransferTicketQuery query)
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

                var transferTicket = await _mediator.Send(query);

                Response.AddPaginationHeader(

                transferTicket.CurrentPage,
                transferTicket.PageSize,
                transferTicket.TotalCount,
                transferTicket.TotalPages,
                transferTicket.HasPreviousPage,
                transferTicket.HasNextPage

                );

                var result = new
                {
                    transferTicket,
                    transferTicket.CurrentPage,
                    transferTicket.PageSize,
                    transferTicket.TotalCount,
                    transferTicket.TotalPages,
                    transferTicket.HasPreviousPage,
                    transferTicket.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("ApprovedTransferTicket")]
        public async Task<IActionResult> ApprovedTransferTicket([FromBody] ApprovedTransferTicketCommand command)
        {
            try
            {
                
                if (User.Identity is ClaimsIdentity identity  )
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }
   
                    if(Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.Transfer_By = userId;
                        command.Users = userId;
                        command.Approver_By = userId;
                        command.Requestor_By = userId;
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

        [HttpPut("RejectTransferTicket")]
        public async Task<IActionResult> RejectTransferTicket([FromBody] RejectTransferTicketCommand command)
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
                        command.RejectTransfer_By = userId;
                        command.Approver_By = userId;
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
