// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Models;

public sealed class MailProviderRegistration
{
    public string Name { get; set; }

    public MicrosoftGraphProviderConfiguration MicrosoftGraph { get; set; }
}