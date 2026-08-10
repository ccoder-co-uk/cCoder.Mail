// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Exposures;

namespace cCoder.Mail.Services.Orchestrations;

internal interface IMailSenderOrchestrationService : IMailDispatchManager
{
    ValueTask<bool> ExistsAsync(Guid mailSenderId);
}