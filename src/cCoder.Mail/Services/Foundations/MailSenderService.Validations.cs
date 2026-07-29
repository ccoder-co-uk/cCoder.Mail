// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Foundations;

internal partial class MailSenderService
{
    private static void ValidateMailSenderOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllMailSenderOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailSenderOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailSenderOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllMailSenderOnDelete(object[] inputs) =>
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