using cCoder.Mail.Models;
using cCoder.Mail.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Exposures.Controllers;

[ApiController]
[Route("Api/Mail/ReceivedEmail")]
[Route("Api/Core/ReceivedEmail")]
public sealed class ReceivedEmailOperationsController(
    IReceivedEmailOrchestrationService service)
    : ControllerBase
{
    [HttpPost("Receive")]
    public async Task<IActionResult> Receive(
        [FromBody] MailboxReceiveRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(await service.ReceiveAsync(request, cancellationToken));
    }

    [HttpGet("ReceiveTop/{count:int}")]
    public async Task<IActionResult> ReceiveTop(
        [FromRoute] int count,
        CancellationToken cancellationToken)
    {
        if (count <= 0)
            return BadRequest("Count must be greater than zero.");

        return Ok(await service.ReceiveTopAsync(count, cancellationToken));
    }
}
