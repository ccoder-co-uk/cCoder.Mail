// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Providers.Services.Foundations;

public interface ISmtpMailSenderService
{
    Task SendQueuedEmailAsync(QueuedEmail email, CancellationToken cancellationToken = default);
}