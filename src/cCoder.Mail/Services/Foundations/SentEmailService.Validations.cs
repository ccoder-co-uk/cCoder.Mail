// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using System.Security;

namespace cCoder.Mail.Services.Foundations;

internal partial class SentEmailService
{
    private static void Authorize(User user, int? appId, string privilege)
    {
        if (!user.Can(appId: appId, operation: privilege))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    private static void ValidateSentEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllSentEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSentEmailOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateSentEmailOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllForAppSentEmailOnDelete(object[] inputs) =>
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