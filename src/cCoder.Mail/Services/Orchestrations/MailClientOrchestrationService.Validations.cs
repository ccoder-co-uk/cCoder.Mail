// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies;

namespace cCoder.Mail.Services.Orchestrations;

internal sealed partial class MailClientOrchestrationService
{
    private static void ValidateSendAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateReceiveAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateReceiveTopAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}