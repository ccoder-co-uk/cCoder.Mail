// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Orchestrations;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using FluentAssertions;
using Mail.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit.Abstractions;

namespace Mail.IntegrationTests.Tests;

public sealed partial class MailDeliveryTests(ITestOutputHelper output)
{
    private const string MailServerName = "Integration";
    private const string CoreConnectionVariableName = "CCODER_ACCEPTANCE_CORE_CONNECTION_STRING";
    private const string SsoConnectionVariableName = "CCODER_ACCEPTANCE_SSO_CONNECTION_STRING";

    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };
    private static IConfigurationRoot TestConfiguration { get; } =
        new ConfigurationBuilder()
            .SetBasePath(basePath: AppContext.BaseDirectory)
        .AddJsonFile(path: "appsettings.testing.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    private ITestOutputHelper Output { get; } = output;

    private static async Task<IntegrationApplication> StartApplicationAsync(IntegrationSettings settings)
    {
        IntegrationWebApplicationFactory factory = new(settings: settings);
        IntegrationDatabaseManager databaseManager = new(services: factory.Services);

        await databaseManager.ResetDatabasesAsync();

        IntegrationSeed seed = await SeedAsync(services: factory.Services, settings: settings);

        HttpClient client = factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(uriString: "https://localhost"),
        });

        return new IntegrationApplication(factory: factory, databaseManager: databaseManager, client: client, appId: seed.AppId, mailSenderId: seed.MailSenderId);
    }

    private async Task<QueuedEmail> QueueEmailAsync(
        HttpClient client,
        int appId,
        Guid mailSenderId,
        string subject,
        string content,
        string to)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
requestUri: "/Api/Core/QueuedEmail",
value: new
{
    appId,
    sentByUserId = "Guest",
    subject,
    content,
    to,
    cc = string.Empty,
    isBodyHtml = false,
    mailServerName = MailServerName,
    mailSenderId,
});

        string responseContent = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: responseContent);

        return JsonSerializer.Deserialize<QueuedEmail>(json: responseContent, options: JsonOptions)
            ?? throw new InvalidOperationException(message: "Expected queued email payload.");
    }

    private static async Task DispatchQueuedMailAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        IMailSenderOrchestrationService mailSender =
            scope.ServiceProvider.GetRequiredService<IMailSenderOrchestrationService>();

        await mailSender.RunAsync();
    }

    private static async Task<IReadOnlyList<SentEmail>> GetSentEmailsAsync(HttpClient client, string subject)
    {
        using HttpResponseMessage response = await client.GetAsync(
requestUri: $"/Api/Core/SentEmail?$top=10&$filter={Uri.EscapeDataString(stringToEscape: $"Subject eq '{ODataString(value: subject)}'")}");

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<SentEmail>>(json: content, options: JsonOptions)?.Value
            ?? throw new InvalidOperationException(message: "Expected sent email OData payload.");
    }

    private async Task<ReceivedEmail> ReceiveEmailAsync(
        HttpClient client,
        IntegrationSettings settings,
        string subject,
        string content,
        DateTimeOffset from)
    {
        DateTimeOffset deadline = DateTimeOffset.UtcNow.Add(timeSpan: settings.ReceiveTimeout);
        List<string> observedSubjects = [];

        do
        {
            ReceivedEmail[] receivedEmails = await ReceiveEmailsAsync(client: client, settings: settings, from: from);

            ReceivedEmail receivedEmail = receivedEmails.FirstOrDefault(predicate: email =>
                string.Equals(a: email.Subject, b: subject, comparisonType: StringComparison.Ordinal)
                && (email.Content ?? string.Empty).Contains(value: content, comparisonType: StringComparison.Ordinal));

            if (receivedEmail is not null)
                return receivedEmail;

            observedSubjects = [.. receivedEmails.Select(selector: email => email.Subject)
                .Where(predicate: value => !string.IsNullOrWhiteSpace(value: value))];

            await Task.Delay(delay: settings.ReceivePollDelay);
        }
        while (DateTimeOffset.UtcNow < deadline);

        throw new InvalidOperationException(
message: $"The sent email was not received within {settings.ReceiveTimeout}. " +
            $"Observed subjects: {string.Join(separator: ", ", values: observedSubjects)}");
    }

    private static async Task<ReceivedEmail[]> ReceiveEmailsAsync(
        HttpClient client,
        IntegrationSettings settings,
        DateTimeOffset from)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
requestUri: "/Api/Core/ReceivedEmail/Receive",
value: new MailboxReceiveRequest
{
    User = settings.ReceiveUser,
    From = from,
    To = DateTimeOffset.UtcNow.AddMinutes(minutes: 5),
    MaximumMessages = settings.MaximumMessages,
});

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ReceivedEmail[]>(json: content, options: JsonOptions)
            ?? throw new InvalidOperationException(message: "Expected received email payload.");
    }

    private static async Task<ReceivedEmail[]> ReceiveTopEmailsAsync(
        HttpClient client,
        int count)
    {
        using HttpResponseMessage response = await client.GetAsync(requestUri: $"/Api/Core/ReceivedEmail/ReceiveTop/{count}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ReceivedEmail[]>(json: content, options: JsonOptions)
            ?? throw new InvalidOperationException(message: "Expected received email payload.");
    }

    private static async Task<IntegrationSeed> SeedAsync(IServiceProvider services, IntegrationSettings settings)
    {
        using IServiceScope scope = services.CreateScope();

        using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        App app = new()
        {
            Name = "Mail Integration",
            Domain = "mail.integration.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = "mail-integration",
            ConfigJson = "{}",
        };

        core.Set<App>()
            .Add(entity: app);

        await core.SaveChangesAsync();

        core.Set<User>()
            .Add(entity: new User
            {
                Id = "Guest",
                DefaultCultureId = string.Empty,
                DisplayName = "Guest",
                Email = settings.To,
                IsActive = true,
            });

        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = "Mail Integration Administrators",
            Description = "Mail integration bootstrap role",
            Privs = "app_admin,mailserver_create,mailserver_read,mailserver_update,mailserver_delete,"
                + "mailsender_create,mailsender_read,mailsender_update,mailsender_delete,"
                + "mailreceiver_create,mailreceiver_read,mailreceiver_update,mailreceiver_delete,"
                + "queuedemail_create,queuedemail_read,queuedemail_update,queuedemail_delete,"
                + "sentemail_create,sentemail_read,sentemail_update,sentemail_delete,"
                + "receivedemail_create,receivedemail_read,receivedemail_update,receivedemail_delete",
        };

        core.Set<Role>()
            .Add(entity: role);

        core.Set<UserRole>()
            .Add(entity: new UserRole { RoleId = role.Id, UserId = "Guest" });

        MailSender mailSender = new()
        {
            AppId = app.Id,
            Name = MailServerName,
            ProviderName = MailProviderNames.MicrosoftGraph,
            Host = settings.SendHost,
            Port = 443,
            EnableSSL = true,
            User = settings.SendUser,
            Password = string.Empty,
            FromEmail = settings.From,
        };

        core.Set<MailSender>()
            .Add(entity: mailSender);

        await core.SaveChangesAsync();
        return new IntegrationSeed(AppId: app.Id, MailSenderId: mailSender.Id);
    }

    private static IntegrationSettings ReadSettings()
    {
        string sendHost = ReadRequired(variableName: "CCODER_MAIL_INTEGRATION_SEND_HOST");

        string sendUser = ReadRequired(
variableName: "CCODER_MAIL_INTEGRATION_SEND_USER",
fallbackVariableName: "CCODER_MAIL_INTEGRATION_SMTP_USER");

        string receiveUser = ReadRequired(
variableName: "CCODER_MAIL_INTEGRATION_RECEIVE_USER",
fallbackVariableName: "CCODER_MAIL_INTEGRATION_SEND_USER");

        string to = ReadRequired(variableName: "CCODER_MAIL_INTEGRATION_TO");
        string from = ReadRequired(variableName: "CCODER_MAIL_INTEGRATION_SMTP_FROM");
        receiveUser = string.IsNullOrWhiteSpace(value: receiveUser) ? sendUser : receiveUser;

        return new()
        {
            CoreConnectionString = AddDatabaseSuffix(variableName: CoreConnectionVariableName, suffix: "mail-integration"),
            SsoConnectionString = AddDatabaseSuffix(variableName: SsoConnectionVariableName, suffix: "mail-integration"),
            SendHost = string.IsNullOrWhiteSpace(value: sendHost) ? "graph.microsoft.com" : sendHost,
            SendUser = sendUser,
            From = string.IsNullOrWhiteSpace(value: from) ? sendUser : from,
            ReceiveUser = receiveUser,
            To = string.IsNullOrWhiteSpace(value: to) ? receiveUser : to,
            MaximumMessages = ReadInt(variableName: "CCODER_MAIL_INTEGRATION_MAX_MESSAGES", defaultValue: 50),
            ReceiveTimeout = TimeSpan.FromSeconds(seconds: ReadInt(variableName: "CCODER_MAIL_INTEGRATION_RECEIVE_TIMEOUT_SECONDS", defaultValue: 120)),
            ReceivePollDelay = TimeSpan.FromSeconds(seconds: ReadInt(variableName: "CCODER_MAIL_INTEGRATION_RECEIVE_POLL_SECONDS", defaultValue: 10)),
        };
    }

    private static string ODataString(string value) =>
        (value ?? string.Empty).Replace(oldValue: "'", newValue: "''", comparisonType: StringComparison.Ordinal);

    private static string AddDatabaseSuffix(string variableName, string suffix)
    {
        string connectionString = ReadRequired(variableName: variableName);

        if (string.IsNullOrWhiteSpace(value: connectionString))
            return string.Empty;

        SqlConnectionStringBuilder builder = new(connectionString: connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        if (string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
            return builder.ConnectionString;

        builder.InitialCatalog = $"{builder.InitialCatalog}-{suffix}";
        return builder.ConnectionString;
    }

    private static string ReadRequired(string variableName, string fallbackVariableName = null)
    {
        string value = TestConfiguration[variableName];

        if (!string.IsNullOrWhiteSpace(value: value))
            return value;

        return fallbackVariableName is null ? string.Empty : ReadRequired(variableName: fallbackVariableName);
    }

    private static bool ReadBool(string variableName, bool defaultValue = false)
    {
        string value = ReadRequired(variableName: variableName);

        if (string.IsNullOrWhiteSpace(value: value))
            return defaultValue;

        if (int.TryParse(s: value, result: out int numericValue))
            return numericValue != 0;

        return bool.Parse(value: value);
    }

    private static int ReadInt(string variableName, int defaultValue)
    {
        string value = ReadRequired(variableName: variableName);
        return string.IsNullOrWhiteSpace(value: value) ? defaultValue : int.Parse(s: value);
    }

    private sealed record ODataEnvelope<T>(List<T> Value);

    private sealed class IntegrationSettings
    {
        public string CoreConnectionString { get; init; }

        public string SsoConnectionString { get; init; }

        public string SendHost { get; init; }

        public string SendUser { get; init; }

        public string From { get; init; }

        public string ReceiveUser { get; init; }

        public string To { get; init; }

        public int MaximumMessages { get; init; }

        public TimeSpan ReceiveTimeout { get; init; }

        public TimeSpan ReceivePollDelay { get; init; }

        public string[] MissingVariables() =>
            [
            .. RequiredVariableNames()
            .Where(predicate: name => string.IsNullOrWhiteSpace(value: ReadRequired(variableName: name))),
            .. string.IsNullOrWhiteSpace(value: ReadRequired(
variableName: "CCODER_MAIL_INTEGRATION_SEND_USER",
fallbackVariableName: "CCODER_MAIL_INTEGRATION_SMTP_USER"))
                ? ["CCODER_MAIL_INTEGRATION_SEND_USER or CCODER_MAIL_INTEGRATION_SMTP_USER"]
                : Array.Empty<string>(),
        ];

        public static string RequiredVariableSummary() =>
            string.Join(separator: ", ", value: RequiredVariableNames());

        private static string[] RequiredVariableNames() =>
            [
            CoreConnectionVariableName,
            SsoConnectionVariableName,
            "CCODER_MAIL_GRAPH_TENANT_ID",
            "CCODER_MAIL_GRAPH_CLIENT_ID",
            "CCODER_MAIL_GRAPH_CLIENT_SECRET",
        ];
    }

    private sealed record IntegrationSeed(int AppId, Guid MailSenderId);

    private sealed class IntegrationApplication(
        IntegrationWebApplicationFactory factory,
        IntegrationDatabaseManager databaseManager,
        HttpClient client,
        int appId,
        Guid mailSenderId)
        : IAsyncDisposable
    {
        public IntegrationWebApplicationFactory Factory { get; } = factory;

        public HttpClient Client { get; } = client;

        public int AppId { get; } = appId;

        public Guid MailSenderId { get; } = mailSenderId;

        public async ValueTask DisposeAsync()
        {
            Client.Dispose();
            await databaseManager.DropDatabasesAsync();
            await Factory.DisposeAsync();
        }
    }

    private sealed class IntegrationWebApplicationFactory(IntegrationSettings settings)
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
                    new KeyValuePair<string, string>(key: "Settings:DecryptionKey", value: "000000000000000000000000000000000000000000000000"),
                    new KeyValuePair<string, string>(key: "Settings:enableExternalEventing", value: "false"),
                ]);
            });

            builder.ConfigureTestServices(servicesConfiguration: services =>
            {
                services.RemoveAll<ICoreContextFactory>();
                services.RemoveAll<ISecurityDbContextFactory>();

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
        ["DecryptionKey"] = "000000000000000000000000000000000000000000000000",
        ["enableExternalEventing"] = "false",
    },
    Services = new Dictionary<string, string>(),
});

                services.AddSingleton<ISecurityDbContextFactory>(
                    _ => new MSSQLSecurityDbContextFactory(settings.SsoConnectionString));

                services.AddCoreData(connectionString: settings.CoreConnectionString);
            });
        }
    }

    private sealed class IntegrationDatabaseManager(IServiceProvider services)
    {
        public Task ResetDatabasesAsync()
        {
            using IServiceScope scope = services.CreateScope();

            using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>()
                .CreateDbContext(ignoreAuthInfo: true);

            using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            EnsureSafeIntegrationDatabase(connectionString: sso.Database.GetConnectionString(), protectedDatabaseName: "dev-Members");
            EnsureSafeIntegrationDatabase(connectionString: core.Database.GetConnectionString(), protectedDatabaseName: "dev-Core");

            ForceDropDatabase(connectionString: sso.Database.GetConnectionString());
            ForceDropDatabase(connectionString: core.Database.GetConnectionString());

            sso.Migrate();
            core.Migrate();

            return Task.CompletedTask;
        }

        public Task DropDatabasesAsync()
        {
            using IServiceScope scope = services.CreateScope();

            using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>()
                .CreateDbContext(ignoreAuthInfo: true);

            using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            EnsureSafeIntegrationDatabase(connectionString: sso.Database.GetConnectionString(), protectedDatabaseName: "dev-Members");
            EnsureSafeIntegrationDatabase(connectionString: core.Database.GetConnectionString(), protectedDatabaseName: "dev-Core");

            ForceDropDatabase(connectionString: sso.Database.GetConnectionString());
            ForceDropDatabase(connectionString: core.Database.GetConnectionString());

            return Task.CompletedTask;
        }

        private static void EnsureSafeIntegrationDatabase(string connectionString, string protectedDatabaseName)
        {
            if (string.IsNullOrWhiteSpace(value: connectionString))
                throw new InvalidOperationException(message: "Integration database connection string is empty.");

            SqlConnectionStringBuilder builder = CreateConnectionStringBuilder(connectionString: connectionString);
            string databaseName = builder.InitialCatalog ?? string.Empty;

            if (string.IsNullOrWhiteSpace(value: databaseName))
                throw new InvalidOperationException(message: "Integration database name is empty.");

            if (databaseName.Equals(value: protectedDatabaseName, comparisonType: StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
message: $"Refusing to run integration database operations against protected database '{protectedDatabaseName}'.");

            if (!databaseName.Contains(value: "integration", comparisonType: StringComparison.OrdinalIgnoreCase)
                && !databaseName.Contains(value: "accept", comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
message: $"Refusing to run integration database operations against non-integration database '{databaseName}'.");
            }
        }

        private static void ForceDropDatabase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(value: connectionString))
                return;

            SqlConnectionStringBuilder builder = CreateConnectionStringBuilder(connectionString: connectionString);
            string databaseName = builder.InitialCatalog ?? string.Empty;

            if (string.IsNullOrWhiteSpace(value: databaseName))
                return;

            builder.InitialCatalog = "master";

            using SqlConnection connection = new(connectionString: builder.ConnectionString);
            connection.Open();

            using SqlCommand command = connection.CreateCommand();

            command.CommandText = @"
IF DB_ID(@databaseName) IS NOT NULL
BEGIN
    DECLARE @sql nvarchar(max) =
        N'ALTER DATABASE [' + REPLACE(@databaseName, ']', ']]') + N'] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;'
        + N'DROP DATABASE [' + REPLACE(@databaseName, ']', ']]') + N']';
    EXEC(@sql);
END";

            _ = command.Parameters.AddWithValue(parameterName: "@databaseName", value: databaseName);
            command.ExecuteNonQuery();
        }

        private static SqlConnectionStringBuilder CreateConnectionStringBuilder(string connectionString) =>
            new(connectionString: connectionString)
            {
                Encrypt = true,
                TrustServerCertificate = true,
            };
    }
}
