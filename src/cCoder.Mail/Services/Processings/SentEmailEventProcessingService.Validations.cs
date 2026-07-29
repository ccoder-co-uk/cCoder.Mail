// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Processings;

internal partial class SentEmailEventProcessingService
{
    private static void ValidateRaiseSentEmailAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseSentEmailUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseSentEmailDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}