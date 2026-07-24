// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class SmtpMailSenderService
{
    private static void ValidateSendAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}