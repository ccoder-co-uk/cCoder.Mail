// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Exposures.Controllers;

[ApiController]
[Route("Api/Mail/ReceivedEmail")]
public sealed class ReceivedEmailOperationsController(
    IMailReceivingService service)
    : ControllerBase
{
    [HttpPost("Receive")]
    public async Task<IActionResult> Post(
        [FromBody] MailboxReceiveRequest newMailboxReceiveRequest,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(modelState: ModelState);
        }

        return Ok(value: await service.ReceiveMailboxReceiveRequestAsync(request: newMailboxReceiveRequest, cancellationToken: cancellationToken));
    }

    [HttpGet("ReceiveTop/{mailReceiverId:guid}/{count:int}")]
    public async Task<IActionResult> Get(
        [FromRoute] Guid mailReceiverId,
        [FromRoute] int count,
        CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return BadRequest(error: "Count must be greater than zero.");
        }

        return Ok(
            value: await service.ReceiveTopAsync(
                mailReceiverId: mailReceiverId,
                count: count,
                cancellationToken: cancellationToken));
    }
}