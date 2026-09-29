// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal interface IMicrosoftGraphMailboxService
{
    Task<MicrosoftGraphMailboxRequest> RetrieveMicrosoftGraphMailboxRequestAsync(
        MicrosoftGraphMailboxRequest microsoftGraphMailboxRequest,
        CancellationToken cancellationToken = default);
}