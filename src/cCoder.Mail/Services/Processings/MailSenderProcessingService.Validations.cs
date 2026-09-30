// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.Mail.Services.Processings;

internal partial class MailSenderProcessingService
{
    private static void ValidateMailSenderOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllMailSendersOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailSenderOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMailSenderOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllMailSenderOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}