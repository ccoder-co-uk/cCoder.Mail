// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.Core.Services.Tests.Mail;

internal static class AuthorizationTestUsers
{
    internal static User CreateAuthorized(
        int? appId,
        string privilege) =>
        new()
        {
            Roles =
            [
                new UserRole
                {
                    Role = new Role
                    {
                        AppId = appId ?? 0,
                        Privileges = [privilege.ToLowerInvariant()]
                    }
                }
            ]
        };

    internal static User CreateUnauthorized() =>
        new()
        {
            Roles = []
        };
}