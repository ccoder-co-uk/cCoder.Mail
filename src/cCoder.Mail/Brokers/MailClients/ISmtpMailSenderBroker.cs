// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

public interface ISmtpMailSenderBroker
{
    Task SendAsync(SmtpMailSendRequest request, CancellationToken cancellationToken = default);
}