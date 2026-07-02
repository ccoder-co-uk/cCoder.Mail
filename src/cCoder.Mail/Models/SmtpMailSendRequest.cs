namespace cCoder.Mail.Models;

public sealed class SmtpMailSendRequest
{
    public string Host { get; set; }

    public int Port { get; set; }

    public bool EnableSsl { get; set; }

    public string User { get; set; }

    public string Password { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string CC { get; set; }

    public string Subject { get; set; }

    public string Content { get; set; }

    public bool IsBodyHtml { get; set; }
}
