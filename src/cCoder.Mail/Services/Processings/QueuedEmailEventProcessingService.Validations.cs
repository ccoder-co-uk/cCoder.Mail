// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Processings;

internal partial class QueuedEmailEventProcessingService
{
    private static void ValidateRaiseQueuedEmailAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseQueuedEmailUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseQueuedEmailDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}