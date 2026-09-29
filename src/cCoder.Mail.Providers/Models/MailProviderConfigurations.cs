// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
namespace cCoder.Mail.Providers.Models;

public sealed class MailProviderConfigurations
    : Dictionary<string, MailProviderConfiguration>
{
    public MailProviderConfigurations()
        : base(comparer: StringComparer.OrdinalIgnoreCase)
    {
    }
}