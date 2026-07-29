// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Exposures.MailClients;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Exposures.Controllers;

[ApiController]
[Route("Api/Mail/MailProviders")]
public sealed class MailProvidersController(
    IMailProviderCatalog providerCatalog)
    : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: providerCatalog.GetSenders()
        .Concat(second: providerCatalog.GetReceivers())
        .ToArray());

    [HttpGet("Senders")]
    public IActionResult GetSenders() =>
        Ok(value: providerCatalog.GetSenders());

    [HttpGet("Receivers")]
    public IActionResult GetReceivers() =>
        Ok(value: providerCatalog.GetReceivers());
}