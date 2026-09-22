// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Exposures;

namespace cCoder.Mail.Services.Foundations;

internal interface IMailReceivingService : IMailReceivingManager
{
    bool IsMigrationInProgress();

    void LogError(Exception exception);
}