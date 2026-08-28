// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Testing;

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
                "CoreData__ConnectionString",
                "SecurityData__ConnectionString",
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