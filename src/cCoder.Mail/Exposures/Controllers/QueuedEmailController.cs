// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

public partial class QueuedEmailController(IQueuedEmailManager service)
    : ODataController
{
    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        try
        {
            await service.DeleteAsync(iQueuedEmailId: key);

            return NoContent();
        }
        catch (MailValidationException)
        {
            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception)
        {
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
            IQueryable<QueuedEmail> result = service.GetAllQueuedEmail()
                .Where(predicate: queuedEmail => queuedEmail.Id == key);

            QueuedEmail queuedEmail = result.FirstOrDefault();

            if (queuedEmail is null)
            {
                return NotFound();
            }

            return Ok(value: SingleResult.Create(queryable: result));
        }
        catch (MailValidationException)
        {
            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception)
        {
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
            return Ok(value: service.GetAllQueuedEmail());
        }
        catch (MailValidationException)
        {
            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail operation failed.");
        }
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            bool isExtendedMetaRequest = Request.Query["extend"] == "true";

            return isExtendedMetaRequest
                ? Ok(value: new MailModelBroker()
                    .Build()
                    .EDMModel
                    .GetExtendedMetadataForType(
                        context: "Mail",
                        type: typeof(QueuedEmail)))
                : Ok(value: typeof(QueuedEmail).CreateMetadataContainer(
                    isEntity: true,
                    hasEndpoint: true));
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail metadata operation failed.");
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
                return BadRequest(modelState: ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await service.AddQueuedEmailAsync(newQueuedEmail: newQueuedEmail));
        }
        catch (MailValidationException)
        {
            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception)
        {
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
                return BadRequest(modelState: ModelState);
            }

            updatedQueuedEmail.Id = key;

            return Ok(value: await service.UpdateQueuedEmailAsync(
                updatedQueuedEmail: updatedQueuedEmail));
        }
        catch (MailValidationException)
        {
            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail operation failed.");
        }
    }

    [AcceptVerbs("PATCH", "MERGE")]
    [ActionName("Patch")]
    public async Task<IActionResult> Put(
        [FromRoute] int key,
        Delta<QueuedEmail> updatedQueuedEmail)
    {
        try
        {
            QueuedEmail originalEntity = service.GetQueuedEmail(iQueuedEmailId: key);

            if (originalEntity is null)
            {
                return NotFound();
            }

            updatedQueuedEmail.Patch(original: originalEntity);

            return Ok(value: await service.UpdateQueuedEmailAsync(
                updatedQueuedEmail: originalEntity));
        }
        catch (MailValidationException)
        {
            return BadRequest(error: "The mail request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status403Forbidden,
                value: "The mail operation is forbidden.");
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The mail operation failed.");
        }
    }
}