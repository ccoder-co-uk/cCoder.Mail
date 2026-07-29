// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailReceivingService
{
    private static void ValidateReceiveMailboxReceiveRequestAsync(object[] inputs) =>
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