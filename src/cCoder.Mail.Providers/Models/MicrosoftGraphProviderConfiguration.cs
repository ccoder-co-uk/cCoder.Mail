// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Models;

public sealed class MicrosoftGraphProviderConfiguration
{
    public string TenantId { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string GraphBaseUrl { get; set; }

    public string LoginBaseUrl { get; set; }
}