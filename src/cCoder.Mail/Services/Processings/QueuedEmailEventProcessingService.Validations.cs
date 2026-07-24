// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies;

namespace cCoder.Mail.Services.Processings;

internal partial class QueuedEmailEventProcessingService
{
    private static void ValidateRaiseQueuedEmailAddEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaiseQueuedEmailUpdateEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRaiseQueuedEmailDeleteEventAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}