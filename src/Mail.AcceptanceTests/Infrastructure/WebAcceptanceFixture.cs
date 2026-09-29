// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using cCoder.Mail.Testing;
using Web.AcceptanceTests.Models;
using Xunit;


namespace Web.AcceptanceTests.Infrastructure;

public sealed class WebAcceptanceFixture : IAsyncLifetime
{
    private readonly List<WebAcceptanceFactory> supplementalFactories = [];
    private AcceptanceDatabaseManager databaseManager;
    private AcceptanceSettings settings;

    internal WebAcceptanceFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        AcceptanceTestConfiguration configuration =
            AcceptanceTestConfiguration.Load();

        settings = new AcceptanceSettings
        {
            CoreConnectionString = configuration.CoreConnectionString,
            SsoConnectionString =
                configuration.SecurityConnectionString,
            DecryptionKey = configuration.SecurityDecryptionKey
        };

        Factory = new WebAcceptanceFactory(settings: settings);
        databaseManager = new AcceptanceDatabaseManager(services: Factory.Services);
        await databaseManager.ResetDatabasesAsync();
        await SeedAsync();

        Client = Factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(uriString: "https://localhost"),
        });
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        foreach (WebAcceptanceFactory factory in supplementalFactories)
        {
            await factory.DisposeAsync();
        }

        if (databaseManager is not null)
        {
            await databaseManager.DropDatabasesAsync();
        }

        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }
    }

    private Task SeedAsync() =>
        new AcceptanceApplicationSeeder(services: Factory.Services).SeedAsync();

    internal HttpClient CreateFailingProviderClient()
    {
        WebAcceptanceFactory factory = new(
            settings: settings,
            includeFailingMailClient: true);

        supplementalFactories.Add(item: factory);

        return factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(uriString: "https://localhost"),
        });
    }

}