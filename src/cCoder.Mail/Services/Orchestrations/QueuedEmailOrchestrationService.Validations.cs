// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.Mail.Services.Orchestrations;

internal partial class QueuedEmailOrchestrationService
{
    private static void ValidateQueuedEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllQueuedEmailsOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateQueuedEmailOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateQueuedEmailOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateQueuedEmailResultsOnAdd(object[] inputs) =>
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