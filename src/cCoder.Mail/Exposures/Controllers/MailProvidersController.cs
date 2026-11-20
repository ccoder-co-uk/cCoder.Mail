// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Models;
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
            MailProviderSummary[] values = providerCatalog.GetSenders()
                .Concat(second: providerCatalog.GetReceivers())
                .ToArray();

            loggingBroker.LogInformation(
                message: "Controller request succeeded.");

            return Ok(value: values);
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);
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
            MailProviderSummary[] values = providerCatalog.GetSenders();

            loggingBroker.LogInformation(
                message: "Controller request succeeded.");

            return Ok(value: values);
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);
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
            MailProviderSummary[] values = providerCatalog.GetReceivers();

            loggingBroker.LogInformation(
                message: "Controller request succeeded.");

            return Ok(value: values);
        }
        catch (MailValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);
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