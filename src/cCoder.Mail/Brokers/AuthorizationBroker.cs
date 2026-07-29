// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Brokers;

public interface IAuthorizationBroker
{
    User GetCurrentUser();
}

internal class AuthorizationBroker(ICoreContextFactory coreContextFactory) : IAuthorizationBroker
{
    public User GetCurrentUser()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.User;
    }

}