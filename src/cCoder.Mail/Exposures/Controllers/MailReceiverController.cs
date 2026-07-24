// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Extensions;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Dependencies.OData;
using cCoder.Mail.Services.Processings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.Mail.Exposures.Controllers;

public partial class MailReceiverController(
    IMailReceiverProcessingService service)
    : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
value: new MailModelBuilder()
                    .Build()
            .EDMModel.GetExtendedMetadataForType(context: "Mail", type: typeof(MailReceiver))
            )
            : Ok(value: new MetadataContainer(type: typeof(MailReceiver), isEntity: true, hasEndpoint: true));
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
    public IActionResult GetAll(ODataQueryOptions<MailReceiver> queryOptions) =>
        Ok(value: service.GetAllMailReceiver());

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
    public IActionResult Get([FromRoute] Guid key)
    {
        try
        {
            IQueryable<MailReceiver> result = service.GetAllMailReceiver()
                .Where(predicate: mailReceiver => mailReceiver.Id == key);

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
    public async Task<IActionResult> Post([FromBody] MailReceiver newMailReceiver)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Mail.Dependencies.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.AddMailReceiverAsync(newMailReceiver: newMailReceiver));
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] MailReceiver updatedMailReceiver)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Mail.Dependencies.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.UpdateMailReceiverAsync(updatedMailReceiver: updatedMailReceiver));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    [ActionName("Patch")]
    public async Task<IActionResult> Put([FromRoute] Guid key, Delta<MailReceiver> updatedMailReceiver)
    {
        MailReceiver originalEntity = service.GetMailReceiver(iMailReceiverId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        updatedMailReceiver.Patch(original: originalEntity);
        return Ok(value: await service.UpdateMailReceiverAsync(updatedMailReceiver: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await service.DeleteAsync(iMailReceiverId: key);
        return Ok();
    }
}