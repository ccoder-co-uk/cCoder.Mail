// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Services.Foundations;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Exposures.Controllers;

[ApiController]
[Route("Api/Mail/ReceivedEmail")]
public sealed class ReceivedEmailOperationsController(
    IMailReceivingManager service,
    ILoggingBroker loggingBroker)
    : ControllerBase
{
    [HttpPost("Receive")]
    public async Task<IActionResult> Post(
        [FromBody] MailboxReceiveRequest newMailboxReceiveRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await service.ReceiveMailboxReceiveRequestAsync(
                    request: newMailboxReceiveRequest,
                    cancellationToken: cancellationToken));
        }
        catch (ArgumentException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The mail receive request is invalid.");
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The mail receive request is invalid.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail receive operation failed.");
        }
    }

    [HttpGet("ReceiveTop/{mailReceiverId:guid}/{count:int}")]
    public async Task<IActionResult> Get(
        [FromRoute] Guid mailReceiverId,
        [FromRoute] int count,
        CancellationToken cancellationToken)
    {
        try
        {
            if (count <= 0)
            {
                return BadRequest(error: "Count must be greater than zero.");
            }

            return Ok(
                value: await service.ReceiveTopAsync(
                    mailReceiverId: mailReceiverId,
                    count: count,
                    cancellationToken: cancellationToken));
        }
        catch (ArgumentException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The mail receive request is invalid.");
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(error: "The mail receive request is invalid.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail receive operation failed.");
        }
    }
}