// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Aggregations;

internal partial class AppAggregationService
{
    private static void ValidateAppOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}