// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models;

public sealed class MailboxReceiveRequest
{
    public MailboxReceiveRequest()
    {
        Port = 995;
        EnableSSL = true;
        MaximumMessages = 100;
    }

    public string ProviderName { get; set; }

    public int? AppId { get; set; }

    public Guid? MailReceiverId { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public bool EnableSSL { get; set; }

    public string User { get; set; }

    public string Password { get; set; }

    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }

    public int MaximumMessages { get; set; }
}