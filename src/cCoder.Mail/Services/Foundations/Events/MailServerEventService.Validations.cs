// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Foundations.Events;

internal partial class MailServerEventService
{
    private static void ValidateRaiseMailServerAddEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseMailServerUpdateEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRaiseMailServerDeleteEventAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}