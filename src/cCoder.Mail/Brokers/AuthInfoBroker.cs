// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;

namespace cCoder.Mail.Brokers;

public interface IAuthInfoBroker
{
    string GetSsoUserId();
}

internal sealed class AuthInfoBroker(ICoreAuthInfo authInfo) : IAuthInfoBroker
{
    public string GetSsoUserId() =>
        authInfo.SSOUserId;
}