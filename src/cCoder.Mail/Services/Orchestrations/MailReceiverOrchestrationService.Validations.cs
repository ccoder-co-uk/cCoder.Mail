// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Orchestrations;

internal sealed partial class MailReceiverOrchestrationService
{
    private static void ValidateMailReceiverOnExists(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailReceiverOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailReceiverOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRunContinuouslyAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRunAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}