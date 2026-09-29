// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Providers.Services.Foundations;

internal interface ISmtpMailSenderService
{
    Task SendQueuedEmailAsync(QueuedEmail queuedEmail, CancellationToken cancellationToken = default);
}