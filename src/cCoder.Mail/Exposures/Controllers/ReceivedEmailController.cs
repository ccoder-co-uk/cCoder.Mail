// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.OData;
using cCoder.Mail.Extensions.OData;
using cCoder.Mail.Models.OData;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Services.Processings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.Mail.Exposures.Controllers;

public partial class ReceivedEmailController(IReceivedEmailProcessingService service,
    ILoggingBroker loggingBroker)
    : ODataController
{
    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        try
        {
            await service.DeleteAsync(iReceivedEmailId: key);

            loggingBroker.LogInformation(
                message: "Controller request completed successfully.");

            return NoContent();
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
            IQueryable<ReceivedEmail> result = service.GetAllReceivedEmails()
                .Where(predicate: receivedEmail => receivedEmail.Id == key);

            ReceivedEmail receivedEmail = result.FirstOrDefault();

            if (receivedEmail is null)
            {
                loggingBroker.LogWarning(message: "Requested mail resource was not found.");

                return NotFound();
            }

            loggingBroker.LogInformation(
                message: "Controller request completed successfully.");

            return Ok(value: receivedEmail);
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
            IQueryable<ReceivedEmail> values = service.GetAllReceivedEmails();

            loggingBroker.LogInformation(
                message: "Controller request completed successfully.");

            return Ok(value: values);
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
    public async Task<IActionResult> Post([FromBody] ReceivedEmail newReceivedEmail)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "The mail request is invalid.");

                return BadRequest(modelState: ModelState);
            }

            ReceivedEmail value = await service.AddReceivedEmailAsync(newReceivedEmail: newReceivedEmail);

            loggingBroker.LogInformation(
                message: "Controller request completed successfully.");

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: value);
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
        [FromBody] ReceivedEmail updatedReceivedEmail)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "The mail request is invalid.");

                return BadRequest(modelState: ModelState);
            }

            updatedReceivedEmail.Id = key;

            ReceivedEmail value = await service.UpdateReceivedEmailAsync(
                updatedReceivedEmail: updatedReceivedEmail);

            loggingBroker.LogInformation(
                message: "Controller request completed successfully.");

            return Ok(value: value);
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