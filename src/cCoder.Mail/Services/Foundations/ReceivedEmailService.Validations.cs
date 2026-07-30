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