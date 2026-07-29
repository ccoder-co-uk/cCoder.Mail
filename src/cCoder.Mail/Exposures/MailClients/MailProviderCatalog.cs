// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Exposures.MailClients;

internal sealed class MailProviderCatalog(
    IEnumerable<IMailClient> mailClients)
    : IMailProviderCatalog
{
    public MailProviderSummary[] GetSenders() =>
        CreateMailProviderSummaryArray(
            providers: mailClients
                .Where(
                    predicate: mailClient =>
                        mailClient.GetSupportedOperations()
                            .Contains(
                                value: MailClientOperation.Send))
                .Select(
                selector: provider =>
                    (provider.GetProviderNames(), "Sender")));

    public MailProviderSummary[] GetReceivers() =>
        CreateMailProviderSummaryArray(
            providers: mailClients
                .Where(
                    predicate: mailClient =>
                        mailClient.GetSupportedOperations()
                            .Contains(
                                value: MailClientOperation.Receive))
                .Select(
                selector: provider =>
                    (provider.GetProviderNames(), "Receiver")));

    private static MailProviderSummary[] CreateMailProviderSummaryArray(
        IEnumerable<(string[] Names, string Direction)> providers) =>
        [
            .. providers.SelectMany(
                selector: provider =>
                    provider.Names.Select(
                        selector: name =>
                            new MailProviderSummary
                            {
                                Name = name,
                                ProviderName = provider.Names[0],
                                Direction = provider.Direction,
                            }))
        ];
}