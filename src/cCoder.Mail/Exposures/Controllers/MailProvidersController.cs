// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Exposures.Controllers;

[ApiController]
[Route("Api/Mail/MailProviders")]
[Route("Api/Core/MailProviders")]
public sealed class MailProvidersController(MailConfiguration mailConfiguration) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: GetSenderProviders()
        .Concat(second: GetReceiverProviders())
        .ToArray());

    [HttpGet("Senders")]
    public IActionResult GetSenders() =>
        Ok(value: GetSenderProviders());

    [HttpGet("Receivers")]
    public IActionResult GetReceivers() =>
        Ok(value: GetReceiverProviders());

    private MailProviderSummary[] GetSenderProviders() =>
        [
        .. mailConfiguration.SenderProviders.Select(selector: provider => new MailProviderSummary
        {
            Name = provider.Key,
            ProviderName = provider.Value,
            Direction = "Sender",
        })
    ];

    private MailProviderSummary[] GetReceiverProviders() =>
        [
        .. mailConfiguration.ReceiverProviders.Select(selector: provider => new MailProviderSummary
        {
            Name = provider.Key,
            ProviderName = provider.Value,
            Direction = "Receiver",
        })
    ];
}