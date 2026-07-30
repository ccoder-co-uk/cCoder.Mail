// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using System.Security;

namespace cCoder.Mail.Services.Foundations;

internal partial class ReceivedEmailService
{
    private static void Authorize(User user, int? appId, string privilege)
    {
        if (!user.Can(appId: appId, operation: privilege))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

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

    private static void ValidateRangeReceivedEmailOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExists(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllReceivedEmailOnDelete(object[] inputs) =>
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