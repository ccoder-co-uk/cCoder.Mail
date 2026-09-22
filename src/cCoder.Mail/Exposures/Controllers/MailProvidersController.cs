// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Services.Foundations;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Exposures.Controllers;

[ApiController]
[Route("Api/Mail/MailProviders")]
public sealed class MailProvidersController(
    IMailProviderCatalogService providerCatalog,
    ILoggingBroker loggingBroker)
    : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok(value: providerCatalog.GetSenders()
                .Concat(second: providerCatalog.GetReceivers())
                .ToArray());
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");
            return BadRequest(error: "Invalid mail provider request.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail provider operation failed.");
        }
    }

    [HttpGet("Senders")]
    public IActionResult GetSenders()
    {
        try
        {
            return Ok(value: providerCatalog.GetSenders());
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");
            return BadRequest(error: "Invalid mail provider request.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail provider operation failed.");
        }
    }

    [HttpGet("Receivers")]
    public IActionResult GetReceivers()
    {
        try
        {
            return Ok(value: providerCatalog.GetReceivers());
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");
            return BadRequest(error: "Invalid mail provider request.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail provider operation failed.");
        }
    }
}