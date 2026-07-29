// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Orchestrations;

internal sealed partial class MailSenderOrchestrationService
{
    private static void ValidateMailSenderOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailSenderOnUpdate(object[] inputs) =>
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