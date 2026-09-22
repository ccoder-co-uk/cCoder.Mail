// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal interface IMicrosoftGraphMailboxService
{
    Task<MicrosoftGraphMailboxRequest> RetrieveMicrosoftGraphMailboxRequestAsync(
        MicrosoftGraphMailboxRequest microsoftGraphMailboxRequest,
        CancellationToken cancellationToken = default);
}