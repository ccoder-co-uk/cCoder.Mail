using HostedServices.AcceptanceTests.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HostedServices.AcceptanceTests.Infrastructure;

public sealed class HostedServicesAcceptanceFixture : IAsyncLifetime
{
    internal HostedServicesAcceptanceFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public Task InitializeAsync()
    {
        AcceptanceSettings settings = new()
        {
            CoreConnectionString = AddDatabaseSuffix("CCODER_ACCEPTANCE_CORE_CONNECTION_STRING"),
        };

        Factory = new HostedServicesAcceptanceFactory(settings);
        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
        });

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (Factory is not null)
            await Factory.DisposeAsync();
    }

    private static string AddDatabaseSuffix(string variableName)
    {
        string connectionString = BuildConfiguration()[variableName]
            ?? ReadConfiguredConnectionString(variableName);

        if (string.IsNullOrWhiteSpace(connectionString))
            return string.Empty;

        SqlConnectionStringBuilder builder = new(connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };
        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(databaseName))
            return connectionString;

        builder.InitialCatalog = $"{databaseName}-mail-hostedservices";
        return builder.ConnectionString;
    }

    private static string ReadConfiguredConnectionString(string variableName)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.testing.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetConnectionString("Core") ?? string.Empty;
    }

    private static IConfigurationRoot BuildConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.testing.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
}

[CollectionDefinition(Name)]
public sealed class HostedServicesAcceptanceCollection : ICollectionFixture<HostedServicesAcceptanceFixture>
{
    public const string Name = "Hosted Services acceptance";
}
