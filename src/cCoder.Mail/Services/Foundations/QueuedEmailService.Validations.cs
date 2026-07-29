// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Foundations;

internal partial class QueuedEmailService
{
    private static void ValidateQueuedEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllQueuedEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDispatchBatchOnGet(object[] inputs) =>
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

    private static void ValidateAllForAppQueuedEmailOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}