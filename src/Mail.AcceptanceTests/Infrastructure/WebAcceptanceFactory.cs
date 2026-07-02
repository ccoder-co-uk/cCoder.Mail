using cCoder.Data;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;
using cCoder.Security.Data.EF;
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
        builder.UseEnvironment("Acceptance");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
            [
                new KeyValuePair<string, string>("ConnectionStrings:Core", settings.CoreConnectionString),
                new KeyValuePair<string, string>("ConnectionStrings:SSO", settings.SsoConnectionString),
                new KeyValuePair<string, string>("Settings:DecryptionKey", settings.DecryptionKey),
                new KeyValuePair<string, string>("Settings:enableExternalEventing", "false"),
            ]);
        });
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<ISecurityDbContextFactory>();
            services.RemoveAll<IMailClient>();
            services.RemoveAll<IMicrosoftGraphClient>();
            services.RemoveAll<IMailSenderProvider>();
            services.RemoveAll<IMailReceiverProvider>();

            services.AddSingleton(
                new cCoder.Data.Config
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
                _ => new MSSQLSecurityDbContextFactory(settings.SsoConnectionString)
            );
            services.AddCoreData(settings.CoreConnectionString);
            services.AddTransient<AcceptanceMailClient>();
            services.AddTransient<IMailClient>(provider => provider.GetRequiredService<AcceptanceMailClient>());
            services.AddTransient<IMicrosoftGraphClient>(provider => provider.GetRequiredService<AcceptanceMailClient>());
            services.AddTransient<IMailSenderProvider, AcceptanceSmtpMailSenderProvider>();
            services.AddTransient<IMailSenderProvider, AcceptanceGraphMailProvider>();
            services.AddTransient<IMailReceiverProvider, AcceptanceGraphMailProvider>();
        });
    }

    private sealed class AcceptanceMailClient : IMailClient, IMicrosoftGraphClient
    {
        public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<ReceivedEmail[]> ReceiveAsync(
            MailboxReceiveRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ReceivedEmail[]>(
            [
                new()
                {
                    MessageId = "<acceptance-message@example.test>",
                    From = request.User,
                    To = "recipient@example.test",
                    Subject = $"Acceptance receive from {request.User}",
                    Content = "Acceptance receive content",
                    IsBodyHtml = false,
                    ReceivedOn = request.From?.AddMinutes(1) ?? DateTimeOffset.UtcNow,
                }
            ]);

        public Task<ReceivedEmail[]> ReceiveTopAsync(
            int count,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ReceivedEmail[]>(
            [
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

    private sealed class AcceptanceSmtpMailSenderProvider(AcceptanceMailClient mailClient) : IMailSenderProvider
    {
        public string ProviderName => MailProviderNames.Smtp;

        public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
            mailClient.SendAsync(email, cancellationToken);
    }

    private sealed class AcceptanceGraphMailProvider(AcceptanceMailClient mailClient)
        : IMailSenderProvider,
            IMailReceiverProvider
    {
        public string ProviderName => MailProviderNames.MicrosoftGraph;

        public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
            mailClient.SendAsync(email, cancellationToken);

        public Task<ReceivedEmail[]> ReceiveAsync(
            MailboxReceiveRequest request,
            CancellationToken cancellationToken = default) =>
            mailClient.ReceiveAsync(request, cancellationToken);

        public Task<ReceivedEmail[]> ReceiveTopAsync(
            int count,
            CancellationToken cancellationToken = default) =>
            mailClient.ReceiveTopAsync(count, cancellationToken);
    }
}



