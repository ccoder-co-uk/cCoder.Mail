// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Mail.Brokers;

public interface IAuthInfoBroker
{
    string GetSsoUserId();
}

internal sealed class AuthInfoBroker(ICoreAuthInfo authInfo)
    : IAuthInfoBroker,
      IUtilityBroker
{
    public string GetSsoUserId() =>
        authInfo.SSOUserId;
}