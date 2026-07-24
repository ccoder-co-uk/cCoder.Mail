// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            CoreConnectionString = AddDatabaseSuffix(variableName: "CCODER_ACCEPTANCE_CORE_CONNECTION_STRING"),
        };

        Factory = new HostedServicesAcceptanceFactory(settings: settings);

        Client = Factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(uriString: "https://localhost"),
        });

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }
    }

    private static string AddDatabaseSuffix(string variableName)
    {
        string connectionString = BuildConfiguration()[variableName]
            ?? ReadConfiguredConnectionString(variableName: variableName);

        if (string.IsNullOrWhiteSpace(value: connectionString))
        {
            return string.Empty;
        }

        SqlConnectionStringBuilder builder = new(connectionString: connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value: databaseName))
        {
            return connectionString;
        }

        builder.InitialCatalog = $"{databaseName}-mail-hostedservices";
        return builder.ConnectionString;
    }

    private static string ReadConfiguredConnectionString(string variableName)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: AppContext.BaseDirectory)
            .AddJsonFile(path: "appsettings.testing.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetConnectionString(name: "Core") ?? string.Empty;
    }

    private static IConfigurationRoot BuildConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(basePath: AppContext.BaseDirectory)
        .AddJsonFile(path: "appsettings.testing.json", optional: true)
        .AddEnvironmentVariables()
        .Build();
}

[CollectionDefinition(Name)]
public sealed class HostedServicesAcceptanceCollection : ICollectionFixture<HostedServicesAcceptanceFixture>
{
    public const string Name = "Hosted Services acceptance";
}