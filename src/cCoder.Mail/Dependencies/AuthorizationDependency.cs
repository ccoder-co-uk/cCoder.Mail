// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Dependencies;

internal static class AuthorizationDependency
{
    internal static void Authorize(
        User user,
        int? appId,
        string privilege)
    {
        if (user == null
            || !(HasAppAdminPrivilege(user: user, appId: appId)
                || HasPrivilege(
                    user: user,
                    appId: appId,
                    privilege: privilege)))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    internal static bool HasAppAdminPrivilege(User user, int? appId) =>
        appId.HasValue
        && (user.Roles?.Any(
            predicate: role =>
                role.Role.AppId == appId.Value
                && role.Role.Allows(
                    user: user,
                    privilege: "app_admin")) ?? false);

    internal static bool HasPrivilege(
        User user,
        int? appId,
        string privilege)
    {
        string normalizedPrivilege = privilege.ToLower();

        return (appId != null
            && HasAppAdminPrivilege(
                user: user,
                appId: appId.Value))
            || (user.Roles?.Any(
                predicate: role =>
                    (appId == null || role.Role.AppId == appId)
                    && role.Role.Privileges.Contains(
                        item: normalizedPrivilege))
                ?? false);
    }
}