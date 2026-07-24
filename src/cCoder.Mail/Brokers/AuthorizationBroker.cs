// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Mail.Brokers;

public interface IAuthorizationBroker
{
    User GetCurrentUser();
    bool IsAdminOfApp(int? appId);
    bool IsAdmin(int appId, string userName);
    void Authorize(int? appId, string privilege);
}

internal class AuthorizationBroker(ICoreContextFactory coreContextFactory) : IAuthorizationBroker
{
    public User GetCurrentUser()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.User;
    }

    public bool IsAdminOfApp(int? appId)
    {
        User user = GetCurrentUser();
        return user != null && appId.HasValue && HasAppAdminPrivilege(user: user, appId: appId.Value);
    }

    public bool IsAdmin(int appId, string userName)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        User user = coreDataContext.Users
            .Include(navigationPropertyPath: foundUser => foundUser.Roles)
            .FirstOrDefault(predicate: foundUser => foundUser.Id == userName);

        App app = coreDataContext.Apps
            .Include(navigationPropertyPath: foundApp => foundApp.Roles.Select(selector: role => role.Users))
            .FirstOrDefault(predicate: foundApp => foundApp.Id == appId);

        return app?.IsAppAdmin(user: user) ?? false;
    }

    public void Authorize(int? appId, string privilege)
    {
        User user = GetCurrentUser();

        if (user == null || !(HasAppAdminPrivilege(user: user, appId: appId) || HasPrivilege(user: user, appId: appId, privilege: privilege)))
            throw new SecurityException(message: "Access Denied!");
    }

    private static bool HasPrivilege(User user, int? appId, string privilege)
    {
        string normalizedPrivilege = privilege.ToLower();

        return (appId != null && HasAppAdminPrivilege(user: user, appId: appId.Value))
            || (user.Roles?.Any(predicate: role =>
                (appId == null || role.Role.AppId == appId)
                && role.Role.Privileges.Contains(item: normalizedPrivilege))
                ?? false);
    }

    private static bool HasAppAdminPrivilege(User user, int? appId) =>
        appId.HasValue
        && (user.Roles?.Any(predicate: role => role.Role.AppId == appId.Value && role.Role.Allows(user: user, privilege: "app_admin")) ?? false);
}