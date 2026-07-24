// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    [HttpGet("ReceiveTop/{count:int}")]
    public async Task<IActionResult> Get(
        [FromRoute] int count,
        CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return BadRequest(error: "Count must be greater than zero.");
        }

        return Ok(value: await service.ReceiveTopAsync(count: count, cancellationToken: cancellationToken));
    }
}