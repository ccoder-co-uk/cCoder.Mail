// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.Mail.Services.Processings;

internal partial class SentEmailProcessingService
{
    private static void ValidateSentEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllSentEmailsOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSentEmailOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSentEmailOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateSentEmailResultsOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllSentEmailOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}