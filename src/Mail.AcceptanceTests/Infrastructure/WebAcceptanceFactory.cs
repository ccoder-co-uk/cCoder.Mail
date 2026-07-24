// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Exposures.MailClients;
using cCoder.Mail.Models;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Mail.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
                new KeyValuePair<string, string>(key: "ConnectionStrings:Core", value: settings.CoreConnectionString),
                new KeyValuePair<string, string>(key: "ConnectionStrings:SSO", value: settings.SsoConnectionString),
                new KeyValuePair<string, string>(key: "Settings:DecryptionKey", value: settings.DecryptionKey),
                new KeyValuePair<string, string>(key: "Settings:enableExternalEventing", value: "false"),
            ]);
        });

        builder.ConfigureTestServices(servicesConfiguration: services =>
        {
            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<ISecurityDbContextFactory>();
            services.RemoveAll<IMicrosoftGraphClient>();
            services.RemoveAll<IMailSenderProvider>();
            services.RemoveAll<IMailReceiverProvider>();

            services.AddSingleton(
implementationInstance: new cCoder.Data.Config
{
    ConnectionStrings = new Dictionary<string, string>
    {
        ["Core"] = settings.CoreConnectionString,
        ["SSO"] = settings.SsoConnectionString,
    },
    Settings = new Dictionary<string, string>
    {
        ["DecryptionKey"] = settings.DecryptionKey,
        ["enableExternalEventing"] = "false",
    },
    Services = new Dictionary<string, string>(),
});

            services.AddSingleton<ISecurityDbContextFactory>(
implementationFactory: _ => new MSSQLSecurityDbContextFactory(connectionString: settings.SsoConnectionString)
            );

            services.AddCoreData(connectionString: settings.CoreConnectionString);
            services.AddTransient<AcceptanceMailClient>();
            services.AddTransient<IMicrosoftGraphClient>(implementationFactory: provider => provider.GetRequiredService<AcceptanceMailClient>());
            services.AddTransient<IMailSenderProvider, AcceptanceSmtpMailSenderProvider>();
            services.AddTransient<IMailSenderProvider, AcceptanceGraphMailProvider>();
            services.AddTransient<IMailReceiverProvider, AcceptanceGraphMailProvider>();
        });
    }

    private sealed class AcceptanceMailClient : IMicrosoftGraphClient
    {
        public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<ReceivedEmail[]> ReceiveAsync(
            MailboxReceiveRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ReceivedEmail[]>(
result: [
                new()
                {
                    MessageId = "<acceptance-message@example.test>",
                    From = request.User,
                    To = "recipient@example.test",
                    Subject = $"Acceptance receive from {request.User}",
                    Content = "Acceptance receive content",
                    IsBodyHtml = false,
                    ReceivedOn = request.From?.AddMinutes(minutes: 1) ?? DateTimeOffset.UtcNow,
                }
            ]);

        public Task<ReceivedEmail[]> ReceiveTopAsync(
            int count,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ReceivedEmail[]>(
result: [
                new()
                {
                    MessageId = "<acceptance-top-message@example.test>",
                    From = "configured@example.test",
                    To = "recipient@example.test",
                    Subject = $"Acceptance top {count}",
                    Content = "Acceptance top receive content",
                    IsBodyHtml = false,
                    ReceivedOn = DateTimeOffset.UtcNow,
                }
            ]);
    }

    private sealed class AcceptanceSmtpMailSenderProvider : IMailSenderProvider
    {
        public string ProviderName =>
            MailProviderNames.Smtp;

        public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class AcceptanceGraphMailProvider(AcceptanceMailClient mailClient)
        : IMailSenderProvider,
            IMailReceiverProvider
    {
        public string ProviderName =>
            MailProviderNames.MicrosoftGraph;

        public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
            mailClient.SendAsync(email: email, cancellationToken: cancellationToken);

        public Task<ReceivedEmail[]> ReceiveAsync(
            MailboxReceiveRequest request,
            CancellationToken cancellationToken = default) =>
            mailClient.ReceiveAsync(request: request, cancellationToken: cancellationToken);

        public Task<ReceivedEmail[]> ReceiveTopAsync(
            int count,
            CancellationToken cancellationToken = default) =>
            mailClient.ReceiveTopAsync(count: count, cancellationToken: cancellationToken);
    }
}