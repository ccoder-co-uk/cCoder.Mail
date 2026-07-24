// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies;

namespace cCoder.Mail.Services.Processings;

internal sealed partial class MailSendingProcessingService
{
    private static void ValidateIsMigrationInProgress(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateLogDispatch(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateLogSummary(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateLogError(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSendQueuedEmailAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}