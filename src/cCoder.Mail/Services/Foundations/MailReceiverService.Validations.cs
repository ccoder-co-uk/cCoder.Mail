// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Foundations;

internal partial class MailReceiverService
{
    private static void ValidateMailReceiverOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllMailReceiverOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateEnabledOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailReceiverOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailReceiverOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllMailReceiverOnDelete(object[] inputs) =>
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