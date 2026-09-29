// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Providers.Brokers.MailClients;

public interface ISmtpMailSenderBroker
{
    Task SendAsync(
        QueuedEmail queuedEmail,
        CancellationToken cancellationToken = default);
}