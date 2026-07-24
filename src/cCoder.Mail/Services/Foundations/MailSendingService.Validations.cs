// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailSendingService
{
    private static void ValidateSendQueuedEmailAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}