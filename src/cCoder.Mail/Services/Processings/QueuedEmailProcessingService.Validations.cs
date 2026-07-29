// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Processings;

internal partial class QueuedEmailProcessingService
{
    private static void ValidateQueuedEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllQueuedEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateGetDispatchBatch(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateQueuedEmailOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateQueuedEmailOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRecordSendFailureAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMarkAsSentQueuedEmailAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateQueuedEmailResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllQueuedEmailOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}