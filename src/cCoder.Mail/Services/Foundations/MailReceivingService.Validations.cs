// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailReceivingService
{
    private static void ValidateReceiveAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateReceiveTopAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}