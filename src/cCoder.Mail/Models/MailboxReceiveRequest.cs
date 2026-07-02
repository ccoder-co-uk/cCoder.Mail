namespace cCoder.Mail.Models;

public sealed class MailboxReceiveRequest
{
    public string ProviderName { get; set; }

    public int? AppId { get; set; }

    public Guid? MailReceiverId { get; set; }

    public string Host { get; set; }

    public int Port { get; set; } = 995;

    public bool EnableSSL { get; set; } = true;

    public string User { get; set; }

    public string Password { get; set; }

    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }

    public int MaximumMessages { get; set; } = 100;
}
