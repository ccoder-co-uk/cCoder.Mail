// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using System.Security;

namespace cCoder.Mail.Services.Foundations;

internal partial class MailReceiverService
{
    private static void Authorize(User user, int? appId, string privilege)
    {
        if (!user.Can(appId: appId, operation: privilege))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

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