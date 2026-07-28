// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using HostedServices.AcceptanceTests.Models;
using cCoder.Mail.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HostedServices.AcceptanceTests.Infrastructure;

public sealed class HostedServicesAcceptanceFixture : IAsyncLifetime
{
    internal HostedServicesAcceptanceFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public Task InitializeAsync()
    {
        AcceptanceTestConfiguration configuration =
            AcceptanceTestConfiguration.Load();

        AcceptanceSettings settings = new()
        {
            CoreConnectionString = configuration.CoreConnectionString
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

}

[CollectionDefinition(Name)]
public sealed class HostedServicesAcceptanceCollection : ICollectionFixture<HostedServicesAcceptanceFixture>
{
    public const string Name = "Hosted Services acceptance";
}