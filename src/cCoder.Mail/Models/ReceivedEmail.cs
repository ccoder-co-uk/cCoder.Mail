namespace cCoder.Mail.Models;

public sealed class ReceivedEmail
{
    public string MessageId { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string CC { get; set; }

    public string Subject { get; set; }

    public string Content { get; set; }

    public bool IsBodyHtml { get; set; }

    public DateTimeOffset? ReceivedOn { get; set; }
}
