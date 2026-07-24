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

public partial class MailSenderController(
    IMailSenderProcessingService service)
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
            .EDMModel.GetExtendedMetadataForType(context: "Mail", type: typeof(MailSender))
            )
            : Ok(value: new MetadataContainer(type: typeof(MailSender), isEntity: true, hasEndpoint: true));
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
    public IActionResult GetAll(ODataQueryOptions<MailSender> queryOptions) =>
        Ok(value: service.GetAllMailSender());

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
            IQueryable<MailSender> result = service.GetAllMailSender()
                .Where(predicate: mailSender => mailSender.Id == key);

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
    public async Task<IActionResult> Post([FromBody] MailSender newMailSender)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Mail.Dependencies.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.AddMailSenderAsync(newMailSender: newMailSender));
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] MailSender updatedMailSender)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Mail.Dependencies.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.UpdateMailSenderAsync(updatedMailSender: updatedMailSender));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Put([FromRoute] Guid key, Delta<MailSender> updatedMailSender)
    {
        MailSender originalEntity = service.GetMailSender(iMailSenderId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        updatedMailSender.Patch(original: originalEntity);
        return Ok(value: await service.UpdateMailSenderAsync(updatedMailSender: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await service.DeleteAsync(iMailSenderId: key);
        return Ok();
    }
}