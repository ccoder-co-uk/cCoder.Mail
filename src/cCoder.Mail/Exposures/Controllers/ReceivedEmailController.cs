using cCoder.Mail.Models;
using cCoder.Mail.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Exposures.Controllers;

[ApiController]
[Route("Api/Mail/[controller]")]
[Route("Api/Core/[controller]")]
public sealed class ReceivedEmailController(
    IMailClientOrchestrationService mailClientOrchestrationService)
    : ControllerBase
{
    [HttpPost("Receive")]
    public async Task<IActionResult> Receive(
        [FromBody] MailboxReceiveRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(await mailClientOrchestrationService.ReceiveAsync(request, cancellationToken));
    }
}
