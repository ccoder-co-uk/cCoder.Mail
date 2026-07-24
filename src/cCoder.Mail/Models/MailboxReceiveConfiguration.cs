// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models;

public sealed class MailboxReceiveConfiguration
{
    public string Host { get; set; }

    public int Port { get; set; }

    public bool EnableSSL { get; set; } = true;

    public string User { get; set; }

    public string Password { get; set; }
}