// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Models;

internal sealed class MicrosoftGraphMailboxRequest
{
    internal MailboxReceiveRequest MailboxReceiveRequest { get; set; }

    internal MailProviderConfiguration MailProviderConfiguration { get; set; }

    internal bool IsSuccessStatusCode { get; set; }

    internal string Content { get; set; }
}