// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using System.Security;

namespace cCoder.Mail.Services.Foundations;

internal partial class QueuedEmailService
{
    private static void Authorize(User user, int? appId, string privilege)
    {
        if (!Can(user: user, appId: appId, operation: privilege))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    private static bool Can(
        User user,
        int? appId,
        string operation)
    {
        string normalizedOperation =
            operation?.ToLowerInvariant() ?? string.Empty;

        return user is not null
            && ((appId.HasValue
                    && user.Roles?.Any(predicate: userRole =>
                        userRole.Role?.AppId == appId.Value
                        && userRole.Role.Privileges.Contains(
                            item: "app_admin")) == true)
                || user.Roles?.Any(predicate: userRole =>
                    (!appId.HasValue
                        || userRole.Role?.AppId == appId.Value)
                    && userRole.Role?.Privileges.Contains(
                        item: normalizedOperation) == true) == true);
    }

    private static void ValidateQueuedEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllQueuedEmailOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDispatchBatchOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateQueuedEmailOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateQueuedEmailOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRecordSendFailureAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateMarkAsSentQueuedEmailAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllForAppQueuedEmailOnDelete(object[] inputs) =>
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