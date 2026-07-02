using cCoder.Data.Extensions;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Exposures.OData;
using cCoder.Mail.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.Mail.Exposures.Controllers;

public partial class MailSenderController(
    IMailSenderConfigurationOrchestrationService service)
    : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                new MailModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType("Core", typeof(MailSender))
            )
            : Ok(new MetadataContainer(typeof(MailSender), true, true));
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
    public IActionResult GetAll(ODataQueryOptions<MailSender> queryOptions) => Ok(service.GetAll());

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
            IQueryable<MailSender> result = service.GetAll().Where(mailSender => mailSender.Id == key);
            return Ok(SingleResult.Create(result));
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
    public async Task<IActionResult> Post([FromBody] MailSender entity)
    {
        if (!ModelState.IsValid)
            return new cCoder.Mail.Exposures.OData.BadRequestResult(ModelState);

        return Ok(await service.AddAsync(entity));
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] MailSender entity)
    {
        if (!ModelState.IsValid)
            return new cCoder.Mail.Exposures.OData.BadRequestResult(ModelState);

        return Ok(await service.UpdateAsync(entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<MailSender> delta)
    {
        MailSender originalEntity = service.Get(key);

        if (originalEntity == null)
            return NotFound();

        delta.Patch(originalEntity);
        return Ok(await service.UpdateAsync(originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await service.DeleteAsync(key);
        return Ok();
    }
}
