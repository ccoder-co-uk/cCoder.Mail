// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Orchestrations;

internal sealed partial class ReceivedEmailOrchestrationService
{
    private static void ValidateReceivedEmailOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateReceivedEmailOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateReceiveMailboxReceiveRequestAsync(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateReceiveTopAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}