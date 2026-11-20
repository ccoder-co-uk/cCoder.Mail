// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Exposures.Controllers;

[ApiController]
[Route("Api/Mail/ReceivedEmail")]
public sealed class ReceivedEmailOperationsController(
    IReceivedEmailOrchestrationService service,
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
                loggingBroker.LogWarning(message: "The mail request is invalid.");

                return BadRequest(modelState: ModelState);
            }

            ReceivedEmail[] values = await service.ReceiveMailboxReceiveRequestAsync(
                    request: newMailboxReceiveRequest,
                    cancellationToken: cancellationToken);

            loggingBroker.LogInformation(
                message: "Controller request completed successfully.");

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: values);
        }
        catch (ArgumentException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The mail receive request is invalid.");
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

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
                loggingBroker.LogWarning(message: "The mail receive request is invalid.");

                return BadRequest(error: "Count must be greater than zero.");
            }

            ReceivedEmail[] values = await service.ReceiveTopAsync(
                    mailReceiverId: mailReceiverId,
                    count: count,
                    cancellationToken: cancellationToken);

            loggingBroker.LogInformation(
                message: "Controller request completed successfully.");

            return Ok(value: values);
        }
        catch (ArgumentException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The mail receive request is invalid.");
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

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