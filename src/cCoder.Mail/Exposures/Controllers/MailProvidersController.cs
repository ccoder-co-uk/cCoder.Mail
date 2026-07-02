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
        Ok(GetSenderProviders().Concat(GetReceiverProviders()).ToArray());

    [HttpGet("Senders")]
    public IActionResult GetSenders() => Ok(GetSenderProviders());

    [HttpGet("Receivers")]
    public IActionResult GetReceivers() => Ok(GetReceiverProviders());

    private MailProviderSummary[] GetSenderProviders() =>
    [
        .. mailConfiguration.SenderProviders.Select(provider => new MailProviderSummary
        {
            Name = provider.Key,
            ProviderName = provider.Value,
            Direction = "Sender",
        })
    ];

    private MailProviderSummary[] GetReceiverProviders() =>
    [
        .. mailConfiguration.ReceiverProviders.Select(provider => new MailProviderSummary
        {
            Name = provider.Key,
            ProviderName = provider.Value,
            Direction = "Receiver",
        })
    ];
}
