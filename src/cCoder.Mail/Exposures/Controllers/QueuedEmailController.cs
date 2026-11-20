// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Extensions.Loggings;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.OData;
using cCoder.Mail.Extensions.OData;
using cCoder.Mail.Models.OData;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.Mail.Exposures.Controllers;

public partial class QueuedEmailController(IQueuedEmailOrchestrationService service,
    ILoggingBroker loggingBroker)
    : ODataController
{
    [HttpPost("Api/Mail/QueuedEmail({key})/Retry")]
    public async Task<IActionResult> Post([FromRoute] int key)
    {
        try
        {
            await service.RetryAsync(queuedEmailId: key);

            return loggingBroker.LogSuccessful(
                result: NoContent());
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail operation failed.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        try
        {
            await service.DeleteAsync(iQueuedEmailId: key);

            return loggingBroker.LogSuccessful(
                result: NoContent());
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail operation failed.");
        }
    }

    [HttpGet]
    [EnableQuery(MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<QueuedEmail> result = service.GetAllQueuedEmails()
                .Where(predicate: queuedEmail => queuedEmail.Id == key);

            QueuedEmail queuedEmail = result.FirstOrDefault();

            if (queuedEmail is null)
            {
                loggingBroker.LogWarning(message: "Requested mail resource was not found.");

                return NotFound();
            }

            return loggingBroker.LogSuccessful(
                result: Ok(value: queuedEmail));
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail operation failed.");
        }
    }

    [HttpGet]
    [EnableQuery(MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        try
        {
            return loggingBroker.LogSuccessful(
                result: Ok(value: service.GetAllQueuedEmails()));
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail operation failed.");
        }
    }

    [HttpPost]
    [EnableQuery(MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] QueuedEmail newQueuedEmail)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "The mail request is invalid.");

                return BadRequest(modelState: ModelState);
            }

            return loggingBroker.LogSuccessful(
                result: StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await service.AddQueuedEmailAsync(newQueuedEmail: newQueuedEmail)));
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail operation failed.");
        }
    }

    [HttpPut]
    [EnableQuery(MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put(
        [FromRoute] int key,
        [FromBody] QueuedEmail updatedQueuedEmail)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "The mail request is invalid.");

                return BadRequest(modelState: ModelState);
            }

            updatedQueuedEmail.Id = key;

            return loggingBroker.LogSuccessful(
                result: Ok(value: await service.UpdateQueuedEmailAsync(
                updatedQueuedEmail: updatedQueuedEmail)));
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail operation failed.");
        }
    }

}