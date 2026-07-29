// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Processings;

internal sealed partial class MailReceivingProcessingService
{
    private static void ValidateIsMigrationInProgress(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateLogError(object[] inputs) =>
        Validate(inputs: inputs);

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