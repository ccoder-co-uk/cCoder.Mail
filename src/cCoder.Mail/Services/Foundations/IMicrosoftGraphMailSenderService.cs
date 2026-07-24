// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface IMicrosoftGraphMailSenderService
{
    Task SendQueuedEmailAsync(QueuedEmail email, CancellationToken cancellationToken = default);
}