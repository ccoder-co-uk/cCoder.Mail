// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;

namespace cCoder.Mail.Testing;

internal sealed class AcceptanceTestConfiguration
{
    private AcceptanceTestConfiguration(
        string coreConnectionString,
        string securityConnectionString,
        string securityDecryptionKey)
    {
        CoreConnectionString = coreConnectionString;
        SecurityConnectionString = securityConnectionString;
        SecurityDecryptionKey = securityDecryptionKey;
    }

    internal string CoreConnectionString { get; }
    internal string SecurityConnectionString { get; }
    internal string SecurityDecryptionKey { get; }

    internal static AcceptanceTestConfiguration Load()
    {
        string suffix = $"-acceptance-{Guid.NewGuid():N}";

        return new AcceptanceTestConfiguration(
            coreConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName: "Mail__ConnectionString"),
                suffix: suffix),
            securityConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName: "Security__ConnectionString"),
                suffix: suffix),
            securityDecryptionKey: ReadRequiredValue(
                variableName: "Security__DecryptionKey"));
    }

    private static string AddDatabaseSuffix(
        string connectionString,
        string suffix)
    {
        SqlConnectionStringBuilder builder =
            new(connectionString: connectionString)
            {
                Encrypt = true,
                TrustServerCertificate = true
            };

        if (string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            throw new InvalidOperationException(
                "Acceptance test connection strings must name a database.");
        }

        builder.InitialCatalog = $"{builder.InitialCatalog}{suffix}";
        return builder.ConnectionString;
    }

    internal static string ReadValue(string variableName) =>
        Environment.GetEnvironmentVariable(variable: variableName)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.User)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.Machine)
        ?? string.Empty;

    private static string ReadRequiredValue(string variableName)
    {
        string value = ReadValue(variableName: variableName);

        if (!string.IsNullOrWhiteSpace(value: value))
        {
            return value;
        }

        throw new InvalidOperationException(
            $"Required configuration environment variable '{variableName}' was not found.");
    }
}

internal sealed class MailIntegrationTestConfiguration
{
    private const int DefaultMaximumMessages = 50;
    private const int DefaultReceiveTimeoutSeconds = 120;
    private const int DefaultReceivePollSeconds = 10;

    private MailIntegrationTestConfiguration(
        AcceptanceTestConfiguration acceptanceConfiguration,
        string tenantId,
        string clientId,
        string clientSecret,
        string sendUser,
        string receiveUser)
    {
        CoreConnectionString =
            acceptanceConfiguration.CoreConnectionString;

        SecurityConnectionString =
            acceptanceConfiguration.SecurityConnectionString;

        SecurityDecryptionKey =
            acceptanceConfiguration.SecurityDecryptionKey;

        TenantId = tenantId;
        ClientId = clientId;
        ClientSecret = clientSecret;
        SendUser = sendUser;
        ReceiveUser = receiveUser;
    }

    internal string CoreConnectionString { get; }
    internal string SecurityConnectionString { get; }
    internal string SecurityDecryptionKey { get; }
    internal string TenantId { get; }
    internal string ClientId { get; }
    internal string ClientSecret { get; }
    internal string SendHost => "graph.microsoft.com";
    internal string SendUser { get; }
    internal string From => SendUser;
    internal string ReceiveUser { get; }
    internal string To => ReceiveUser;
    internal int MaximumMessages => DefaultMaximumMessages;
    internal TimeSpan ReceiveTimeout =>
        TimeSpan.FromSeconds(value: DefaultReceiveTimeoutSeconds);

    internal TimeSpan ReceivePollDelay =>
        TimeSpan.FromSeconds(value: DefaultReceivePollSeconds);

    internal static MailIntegrationTestConfiguration Load()
    {
        AcceptanceTestConfiguration acceptanceConfiguration =
            AcceptanceTestConfiguration.Load();

        return new MailIntegrationTestConfiguration(
            acceptanceConfiguration: acceptanceConfiguration,
            tenantId: AcceptanceTestConfiguration.ReadValue(
                variableName: "Mail__MicrosoftGraph__TenantId"),
            clientId: AcceptanceTestConfiguration.ReadValue(
                variableName: "Mail__MicrosoftGraph__ClientId"),
            clientSecret: AcceptanceTestConfiguration.ReadValue(
                variableName: "Mail__MicrosoftGraph__ClientSecret"),
            sendUser: AcceptanceTestConfiguration.ReadValue(
                variableName: "Mail__MicrosoftGraph__SendUser"),
            receiveUser: AcceptanceTestConfiguration.ReadValue(
                variableName: "Mail__MicrosoftGraph__ReceiveUser"));
    }

    internal string[] MissingVariables()
    {
        List<string> missingVariables = [];

        AddMissingVariable(
            missingVariables: missingVariables,
            variableName: "Mail__MicrosoftGraph__TenantId",
            value: TenantId);

        AddMissingVariable(
            missingVariables: missingVariables,
            variableName: "Mail__MicrosoftGraph__ClientId",
            value: ClientId);

        AddMissingVariable(
            missingVariables: missingVariables,
            variableName: "Mail__MicrosoftGraph__ClientSecret",
            value: ClientSecret);

        AddMissingVariable(
            missingVariables: missingVariables,
            variableName: "Mail__MicrosoftGraph__SendUser",
            value: SendUser);

        AddMissingVariable(
            missingVariables: missingVariables,
            variableName: "Mail__MicrosoftGraph__ReceiveUser",
            value: ReceiveUser);

        return [.. missingVariables];
    }

    internal static string RequiredVariableSummary() =>
        string.Join(
            separator: ", ",
            value:
            [
                "Mail__ConnectionString",
                "Security__ConnectionString",
                "Security__DecryptionKey",
                "Mail__MicrosoftGraph__TenantId",
                "Mail__MicrosoftGraph__ClientId",
                "Mail__MicrosoftGraph__ClientSecret",
                "Mail__MicrosoftGraph__SendUser",
                "Mail__MicrosoftGraph__ReceiveUser"
            ]);

    private static void AddMissingVariable(
        List<string> missingVariables,
        string variableName,
        string value)
    {
        if (string.IsNullOrWhiteSpace(value: value))
        {
            missingVariables.Add(item: variableName);
        }
    }
}