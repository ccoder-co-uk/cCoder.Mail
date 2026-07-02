namespace cCoder.Mail.Models;

public sealed class MicrosoftGraphMailConfiguration
{
    public string TenantId { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string GraphBaseUrl { get; set; }

    public string LoginBaseUrl { get; set; }

    public string ReceiveUser { get; set; }
}
