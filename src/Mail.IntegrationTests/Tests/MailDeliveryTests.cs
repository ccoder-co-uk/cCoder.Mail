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

    private ITestOutputHelper Output { get; } = output;

    private static async Task<IntegrationApplication> StartApplicationAsync(IntegrationSettings settings)
    {
        IntegrationWebApplicationFactory factory = new(settings);
        IntegrationDatabaseManager databaseManager = new(factory.Services);

        await databaseManager.ResetDatabasesAsync();

        int appId = await SeedAsync(factory.Services, settings);
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
        });

        return new IntegrationApplication(factory, databaseManager, client, appId);
    }

    private async Task<QueuedEmail> QueueEmailAsync(
        HttpClient client,
        int appId,
        string subject,
        string content,
        string to)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            "/Api/Core/QueuedEmail",
            new
            {
                appId,
                sentByUserId = "Guest",
                subject,
                content,
                to,
                cc = string.Empty,
                isBodyHtml = false,
                mailServerName = MailServerName,
            });

        string responseContent = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, responseContent);

        return JsonSerializer.Deserialize<QueuedEmail>(responseContent, JsonOptions)
            ?? throw new InvalidOperationException("Expected queued email payload.");
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
            $"/Api/Core/SentEmail?$top=10&$filter={Uri.EscapeDataString($"Subject eq '{ODataString(subject)}'")}");

        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        return JsonSerializer.Deserialize<ODataEnvelope<SentEmail>>(content, JsonOptions)?.Value
            ?? throw new InvalidOperationException("Expected sent email OData payload.");
    }

    private async Task<ReceivedEmail> ReceiveEmailAsync(
        HttpClient client,
        IntegrationSettings settings,
        string subject,
        string content,
        DateTimeOffset from)
    {
        DateTimeOffset deadline = DateTimeOffset.UtcNow.Add(settings.ReceiveTimeout);
        List<string> observedSubjects = [];

        do
        {
            ReceivedEmail[] receivedEmails = await ReceiveEmailsAsync(client, settings, from);
            ReceivedEmail receivedEmail = receivedEmails.FirstOrDefault(email =>
                string.Equals(email.Subject, subject, StringComparison.Ordinal)
                && (email.Content ?? string.Empty).Contains(content, StringComparison.Ordinal));

            if (receivedEmail is not null)
                return receivedEmail;

            observedSubjects = [.. receivedEmails.Select(email => email.Subject).Where(value => !string.IsNullOrWhiteSpace(value))];
            await Task.Delay(settings.ReceivePollDelay);
        }
        while (DateTimeOffset.UtcNow < deadline);

        throw new InvalidOperationException(
            $"The sent email was not received within {settings.ReceiveTimeout}. " +
            $"Observed subjects: {string.Join(", ", observedSubjects)}");
    }

    private static async Task<ReceivedEmail[]> ReceiveEmailsAsync(
        HttpClient client,
        IntegrationSettings settings,
        DateTimeOffset from)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            "/Api/Core/ReceivedEmail/Receive",
            new MailboxReceiveRequest
            {
                User = settings.ReceiveUser,
                From = from,
                To = DateTimeOffset.UtcNow.AddMinutes(5),
                MaximumMessages = settings.MaximumMessages,
            });

        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        return JsonSerializer.Deserialize<ReceivedEmail[]>(content, JsonOptions)
            ?? throw new InvalidOperationException("Expected received email payload.");
    }

    private static async Task<int> SeedAsync(IServiceProvider services, IntegrationSettings settings)
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

        core.Set<App>().Add(app);
        await core.SaveChangesAsync();

        core.Set<User>().Add(new User
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
                + "queuedemail_create,queuedemail_read,queuedemail_update,queuedemail_delete,"
                + "sentemail_create,sentemail_read,sentemail_update,sentemail_delete",
        };

        core.Set<Role>().Add(role);
        core.Set<UserRole>().Add(new UserRole { RoleId = role.Id, UserId = "Guest" });
        core.Set<MailServer>().Add(new MailServer
        {
            AppId = app.Id,
            Name = MailServerName,
            Host = settings.SmtpHost,
            Port = settings.SmtpPort,
            EnableSSL = settings.SmtpEnableSsl,
            User = settings.SmtpUser,
            Password = settings.SmtpPassword,
            FromEmail = settings.From,
        });

        await core.SaveChangesAsync();
        return app.Id;
    }

    private static IntegrationSettings ReadSettings()
    {
        string receiveUser = ReadRequired(
            "CCODER_MAIL_INTEGRATION_RECEIVE_USER",
            "CCODER_MAIL_INTEGRATION_SMTP_USER");
        string to = ReadRequired("CCODER_MAIL_INTEGRATION_TO");

        return new()
        {
            CoreConnectionString = AddDatabaseSuffix(CoreConnectionVariableName, "mail-integration"),
            SsoConnectionString = AddDatabaseSuffix(SsoConnectionVariableName, "mail-integration"),
            SmtpHost = ReadRequired("CCODER_MAIL_INTEGRATION_SMTP_HOST"),
            SmtpPort = ReadInt("CCODER_MAIL_INTEGRATION_SMTP_PORT", 587),
            SmtpEnableSsl = ReadBool("CCODER_MAIL_INTEGRATION_SMTP_SSL", true),
            SmtpUser = ReadRequired("CCODER_MAIL_INTEGRATION_SMTP_USER"),
            SmtpPassword = ReadRequired("CCODER_MAIL_INTEGRATION_SMTP_PASSWORD"),
            From = ReadRequired("CCODER_MAIL_INTEGRATION_SMTP_FROM", "CCODER_MAIL_INTEGRATION_SMTP_USER"),
            ReceiveUser = receiveUser,
            To = string.IsNullOrWhiteSpace(to) ? receiveUser : to,
            MaximumMessages = ReadInt("CCODER_MAIL_INTEGRATION_MAX_MESSAGES", 50),
            ReceiveTimeout = TimeSpan.FromSeconds(ReadInt("CCODER_MAIL_INTEGRATION_RECEIVE_TIMEOUT_SECONDS", 120)),
            ReceivePollDelay = TimeSpan.FromSeconds(ReadInt("CCODER_MAIL_INTEGRATION_RECEIVE_POLL_SECONDS", 10)),
        };
    }

    private static string ODataString(string value) =>
        (value ?? string.Empty).Replace("'", "''", StringComparison.Ordinal);

    private static string AddDatabaseSuffix(string variableName, string suffix)
    {
        string connectionString = ReadRequired(variableName);

        if (string.IsNullOrWhiteSpace(connectionString))
            return string.Empty;

        SqlConnectionStringBuilder builder = new(connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
            return builder.ConnectionString;

        builder.InitialCatalog = $"{builder.InitialCatalog}-{suffix}";
        return builder.ConnectionString;
    }

    private static string ReadRequired(string variableName, string fallbackVariableName = null)
    {
        string value =
            Environment.GetEnvironmentVariable(variableName)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);

        if (!string.IsNullOrWhiteSpace(value))
            return value;

        return fallbackVariableName is null ? string.Empty : ReadRequired(fallbackVariableName);
    }

    private static bool ReadBool(string variableName, bool defaultValue = false)
    {
        string value = ReadRequired(variableName);

        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        if (int.TryParse(value, out int numericValue))
            return numericValue != 0;

        return bool.Parse(value);
    }

    private static int ReadInt(string variableName, int defaultValue)
    {
        string value = ReadRequired(variableName);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : int.Parse(value);
    }

    private sealed record ODataEnvelope<T>(List<T> Value);

    private sealed class IntegrationSettings
    {
        public string CoreConnectionString { get; init; }

        public string SsoConnectionString { get; init; }

        public string SmtpHost { get; init; }

        public int SmtpPort { get; init; }

        public bool SmtpEnableSsl { get; init; }

        public string SmtpUser { get; init; }

        public string SmtpPassword { get; init; }

        public string From { get; init; }

        public string ReceiveUser { get; init; }

        public string To { get; init; }

        public int MaximumMessages { get; init; }

        public TimeSpan ReceiveTimeout { get; init; }

        public TimeSpan ReceivePollDelay { get; init; }

        public string[] MissingVariables() =>
        [
            .. RequiredVariableNames().Where(name => string.IsNullOrWhiteSpace(ReadRequired(name)))
        ];

        public static string RequiredVariableSummary() =>
            string.Join(", ", RequiredVariableNames());

        private static string[] RequiredVariableNames() =>
        [
            CoreConnectionVariableName,
            SsoConnectionVariableName,
            "CCODER_MAIL_INTEGRATION_SMTP_HOST",
            "CCODER_MAIL_INTEGRATION_SMTP_USER",
            "CCODER_MAIL_INTEGRATION_SMTP_PASSWORD",
            "CCODER_MAIL_GRAPH_TENANT_ID",
            "CCODER_MAIL_GRAPH_CLIENT_ID",
            "CCODER_MAIL_GRAPH_CLIENT_SECRET",
        ];
    }

    private sealed class IntegrationApplication(
        IntegrationWebApplicationFactory factory,
        IntegrationDatabaseManager databaseManager,
        HttpClient client,
        int appId)
        : IAsyncDisposable
    {
        public IntegrationWebApplicationFactory Factory { get; } = factory;

        public HttpClient Client { get; } = client;

        public int AppId { get; } = appId;

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
            builder.UseEnvironment("Acceptance");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(
                [
                    new KeyValuePair<string, string>("ConnectionStrings:Core", settings.CoreConnectionString),
                    new KeyValuePair<string, string>("ConnectionStrings:SSO", settings.SsoConnectionString),
                    new KeyValuePair<string, string>("Settings:DecryptionKey", "000000000000000000000000000000000000000000000000"),
                    new KeyValuePair<string, string>("Settings:enableExternalEventing", "false"),
                ]);
            });
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ICoreContextFactory>();
                services.RemoveAll<ISecurityDbContextFactory>();
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
                            ["DecryptionKey"] = "000000000000000000000000000000000000000000000000",
                            ["enableExternalEventing"] = "false",
                        },
                        Services = new Dictionary<string, string>(),
                    });
                services.AddSingleton<ISecurityDbContextFactory>(
                    _ => new MSSQLSecurityDbContextFactory(settings.SsoConnectionString));
                services.AddCoreData(settings.CoreConnectionString);
            });
        }
    }

    private sealed class IntegrationDatabaseManager(IServiceProvider services)
    {
        public Task ResetDatabasesAsync()
        {
            using IServiceScope scope = services.CreateScope();
            using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>()
                .CreateDbContext(true);
            using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            EnsureSafeIntegrationDatabase(sso.Database.GetConnectionString(), "dev-Members");
            EnsureSafeIntegrationDatabase(core.Database.GetConnectionString(), "dev-Core");

            ForceDropDatabase(sso.Database.GetConnectionString());
            ForceDropDatabase(core.Database.GetConnectionString());

            sso.Migrate();
            core.Migrate();

            return Task.CompletedTask;
        }

        public Task DropDatabasesAsync()
        {
            using IServiceScope scope = services.CreateScope();
            using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>()
                .CreateDbContext(true);
            using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            EnsureSafeIntegrationDatabase(sso.Database.GetConnectionString(), "dev-Members");
            EnsureSafeIntegrationDatabase(core.Database.GetConnectionString(), "dev-Core");

            ForceDropDatabase(sso.Database.GetConnectionString());
            ForceDropDatabase(core.Database.GetConnectionString());

            return Task.CompletedTask;
        }

        private static void EnsureSafeIntegrationDatabase(string connectionString, string protectedDatabaseName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Integration database connection string is empty.");

            SqlConnectionStringBuilder builder = CreateConnectionStringBuilder(connectionString);
            string databaseName = builder.InitialCatalog ?? string.Empty;

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("Integration database name is empty.");

            if (databaseName.Equals(protectedDatabaseName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"Refusing to run integration database operations against protected database '{protectedDatabaseName}'.");

            if (!databaseName.Contains("integration", StringComparison.OrdinalIgnoreCase)
                && !databaseName.Contains("accept", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Refusing to run integration database operations against non-integration database '{databaseName}'.");
            }
        }

        private static void ForceDropDatabase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return;

            SqlConnectionStringBuilder builder = CreateConnectionStringBuilder(connectionString);
            string databaseName = builder.InitialCatalog ?? string.Empty;

            if (string.IsNullOrWhiteSpace(databaseName))
                return;

            builder.InitialCatalog = "master";

            using SqlConnection connection = new(builder.ConnectionString);
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
            _ = command.Parameters.AddWithValue("@databaseName", databaseName);
            command.ExecuteNonQuery();
        }

        private static SqlConnectionStringBuilder CreateConnectionStringBuilder(string connectionString) =>
            new(connectionString)
            {
                Encrypt = true,
                TrustServerCertificate = true,
            };
    }
}
