// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies.OData;
using cCoder.Mail.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.Mail.Exposures.Controllers;

public partial class QueuedEmailController(
    IQueuedEmailOrchestrationService service)
        : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
value: new cCoder.Mail.Dependencies.OData.MailModelBuilder()
                    .Build()
            .EDMModel.GetExtendedMetadataForType(context: "Mail", type: typeof(QueuedEmail))
            )
            : Ok(value: new MetadataContainer(type: typeof(QueuedEmail), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<QueuedEmail> queryOptions) =>
        Ok(value: service.GetAllQueuedEmail());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 3,
        MaxExpansionDepth = 3
    )]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<QueuedEmail> result = service.GetAllQueuedEmail()
                .Where(predicate: queuedEmail => queuedEmail.Id == key);

            return Ok(value: SingleResult.Create(queryable: result));
        }
        catch (System.Security.SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Post([FromBody] QueuedEmail entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Mail.Dependencies.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.AddQueuedEmailAsync(newQueuedEmail: entity));
    }

    [HttpPut]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] QueuedEmail entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Mail.Dependencies.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.UpdateQueuedEmailAsync(updatedQueuedEmail: entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] int key, Delta<QueuedEmail> delta)
    {
        QueuedEmail originalEntity = service.GetQueuedEmail(iQueuedEmailId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: await service.UpdateQueuedEmailAsync(updatedQueuedEmail: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await service.DeleteAsync(iQueuedEmailId: key);
        return Ok();
    }
}