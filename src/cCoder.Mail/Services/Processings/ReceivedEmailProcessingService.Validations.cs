// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Processings;

internal partial class ReceivedEmailProcessingService
{
    private static void ValidateReceivedEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllReceivedEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateReceivedEmailOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateReceivedEmailOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRangeReceivedEmailOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExists(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllReceivedEmailOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}