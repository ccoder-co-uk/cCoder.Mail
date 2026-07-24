// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using HostedServices.AcceptanceTests.Models;
using Mail.HostedServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace HostedServices.AcceptanceTests.Infrastructure;

internal sealed class HostedServicesAcceptanceFactory(AcceptanceSettings settings)
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
                new KeyValuePair<string, string>(key: "Settings:enableExternalEventing", value: "false"),
                new KeyValuePair<string, string>(key: "MIGRATING", value: "1"),
            ]);
        });
    }
}