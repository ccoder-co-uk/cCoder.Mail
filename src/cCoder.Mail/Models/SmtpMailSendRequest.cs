// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Mail;

namespace cCoder.Mail.Models;

public sealed class SmtpMailSendRequest
{
    public string Host { get; set; }

    public int Port { get; set; }

    public bool EnableSsl { get; set; }

    public string User { get; set; }

    public string Password { get; set; }

    public MailMessage Message { get; set; }
}