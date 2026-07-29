// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Exposures.MailClients;
using cCoder.Mail.Models;
using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Models;
using Mail.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Web.AcceptanceTests.Models;


namespace Web.AcceptanceTests.Infrastructure;

internal sealed class WebAcceptanceFactory(AcceptanceSettings settings)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment: "Acceptance");

        builder.ConfigureAppConfiguration(configureDelegate: (_, config) =>
        {
            config.AddInMemoryCollection(
initialData: [
                new KeyValuePair<string, string>(key: "Mail:ConnectionString", value: settings.CoreConnectionString),
                new KeyValuePair<string, string>(key: "Data:ConnectionString", value: settings.CoreConnectionString),
                new KeyValuePair<string, string>(key: "Security:ConnectionString", value: settings.SsoConnectionString),
                new KeyValuePair<string, string>(key: "Security:DecryptionKey", value: settings.DecryptionKey),
                new KeyValuePair<string, string>(key: "Eventing:ProviderType", value: string.Empty),
            ]);
        });

        builder.ConfigureTestServices(servicesConfiguration: services =>
        {
            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<CoreDataContext>();
            services.RemoveAll<IDbContextFactory<CoreDataContext>>();
            services.RemoveAll<DataConfiguration>();
            services.RemoveAll<ISecurityDbContextFactory>();
            services.RemoveAll<IMailClient>();
            services.RemoveAll<IMailClientFactory>();

            services.AddSingleton<ISecurityDbContextFactory>(
implementationFactory: _ => new MSSQLSecurityDbContextFactory(connectionString: settings.SsoConnectionString)
            );

            services.AddData(
                configuration: new DataConfiguration
                {
                    ConnectionString = settings.CoreConnectionString
                });

            services.AddTransient<AcceptanceMailClient>();
            services.AddTransient<IMailClient, AcceptanceSmtpMailClient>();
            services.AddTransient<IMailClient, AcceptanceGraphMailClient>();
            services.AddTransient<IMailClientFactory, AcceptanceMailClientFactory>();
        });
    }

    private sealed class AcceptanceMailClient
    {
        public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<ReceivedEmail[]> ReceiveAsync(
            Guid mailReceiverId,
            int maximumMessages,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ReceivedEmail[]>(
result: [
                new()
                {
                    MessageId = maximumMessages == 1
                        ? "<acceptance-top-message@example.test>"
                        : "<acceptance-message@example.test>",
                    From = maximumMessages == 1
                        ? "configured@example.test"
                        : "sender@example.test",
                    To = "recipient@example.test",
                    Subject = maximumMessages == 1
                        ? "Acceptance top 1"
                        : "Acceptance receive from sender@example.test",
                    Content = maximumMessages == 1
                        ? "Acceptance top receive content"
                        : "Acceptance receive content",
                    IsBodyHtml = false,
                    ReceivedOn = DateTimeOffset.UtcNow,
                }
            ]);
    }

    private sealed class AcceptanceMailClientFactory(
        IEnumerable<IMailClient> mailClients)
        : IMailClientFactory
    {
        public IMailClient CreateMailClient(string providerName) =>
            mailClients.Single(
                predicate: client =>
                    client.GetProviderNames()
                        .Contains(
                            value: providerName,
                            comparer: StringComparer.OrdinalIgnoreCase));

        public ValueTask<IMailClient> CreateMailClientAsync(
            Guid mailReceiverId,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                result: CreateMailClient(
                    providerName: MailProviderNames.MicrosoftGraph));
    }

    private sealed class AcceptanceSmtpMailClient : IMailClient
    {
        public string[] GetProviderNames() =>
            [MailProviderNames.Smtp];

        public MailClientOperation[] GetSupportedOperations() =>
            [MailClientOperation.Send];

        public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<ReceivedEmail[]> ReceiveAsync(
            Guid mailReceiverId,
            int maximumMessages,
            CancellationToken cancellationToken = default) =>
            throw new UnsupportedMailClientOperationException(
                providerName: MailProviderNames.Smtp,
                operation: MailClientOperation.Receive.ToString());
    }

    private sealed class AcceptanceGraphMailClient(AcceptanceMailClient mailClient)
        : IMailClient
    {
        public string[] GetProviderNames() =>
            [MailProviderNames.MicrosoftGraph];

        public MailClientOperation[] GetSupportedOperations() =>
            [MailClientOperation.Send, MailClientOperation.Receive];

        public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
            mailClient.SendAsync(email: email, cancellationToken: cancellationToken);

        public Task<ReceivedEmail[]> ReceiveAsync(
            Guid mailReceiverId,
            int maximumMessages,
            CancellationToken cancellationToken = default) =>
            mailClient.ReceiveAsync(
                mailReceiverId: mailReceiverId,
                maximumMessages: maximumMessages,
                cancellationToken: cancellationToken);
    }
}