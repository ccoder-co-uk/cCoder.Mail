// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Models;

internal sealed class MicrosoftGraphMessage
{
    public string InternetMessageId { get; set; }

    public MicrosoftGraphRecipient From { get; set; }

    public MicrosoftGraphRecipient[] ToRecipients { get; set; }

    public MicrosoftGraphRecipient[] CcRecipients { get; set; }

    public string Subject { get; set; }

    public MicrosoftGraphBody Body { get; set; }

    public string ReceivedDateTime { get; set; }
}