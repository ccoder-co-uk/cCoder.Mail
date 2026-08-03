// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Models;

public sealed class MailProviderConfigurations
    : Dictionary<string, MailProviderConfiguration>
{
    public MailProviderConfigurations()
        : base(comparer: StringComparer.OrdinalIgnoreCase)
    {
    }
}