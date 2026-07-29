// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Foundations;

internal partial class MailServerService
{
    private static void ValidateMailServerOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllMailServerOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailServerOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailServerOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllForAppMailServerOnDelete(object[] inputs) =>
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