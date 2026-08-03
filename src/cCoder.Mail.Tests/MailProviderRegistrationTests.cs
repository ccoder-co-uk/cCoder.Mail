// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Mail.Models;
using cCoder.Mail.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests;

public sealed partial class MailProviderConfigurationTests
{
    [Fact]
    public void Bind_WithProviderDictionary_BindsProviderConfiguration()
    {
        // Given
        Dictionary<string, string> values = new()
        {
            ["Providers:MicrosoftGraph:TenantId"] =
                "tenant-id"
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                initialData: values!)
            .Build();

        MailConfiguration mailConfiguration = new();

        // When
        configuration.Bind(instance: mailConfiguration);

        // Then
        Assert.True(
            condition: mailConfiguration.Providers.ContainsKey(
                key: MailProviderNames.MicrosoftGraph));

        Assert.Equal(
            expected: "tenant-id",
            actual: mailConfiguration
                .Providers[MailProviderNames.MicrosoftGraph]
                .TenantId);
    }

    [Fact]
    public void AddMailProviders_WithNoProviders_BuildsServiceProvider()
    {
        // Given
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton(
            implementationInstance: Mock.Of<ICoreContextFactory>());

        // When
        services.AddMailProviders(providers: []);

        ServiceProvider serviceProvider = services.BuildServiceProvider(
            options: new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

        // Then
        Assert.NotNull(@object: serviceProvider);
    }
}