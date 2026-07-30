// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Exposures;

public interface IMailDispatchManager
{
    ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender);
    ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender);
    ValueTask DeleteByAppIdAsync(int appId);
    Task RunAsync(CancellationToken cancellationToken = default);
    Task RunContinuouslyAsync(CancellationToken cancellationToken = default);
}
