// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Brokers.MailClients;

public interface IMailSenderClientBroker
{
    Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default);
}