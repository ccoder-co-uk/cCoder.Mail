// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

public interface ISmtpMailSenderService
{
    Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default);
}