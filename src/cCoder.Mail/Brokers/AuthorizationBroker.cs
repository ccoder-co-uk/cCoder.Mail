// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Mail.Dependencies;
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

        return user != null
            && appId.HasValue
            && AuthorizationDependency.HasAppAdminPrivilege(
                user: user,
                appId: appId.Value);
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

        AuthorizationDependency.Authorize(
            user: user,
            appId: appId,
            privilege: privilege);
    }
}