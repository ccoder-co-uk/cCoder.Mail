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
        builder.UseEnvironment("Acceptance");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
            [
                new KeyValuePair<string, string>("ConnectionStrings:Core", settings.CoreConnectionString),
                new KeyValuePair<string, string>("Settings:enableExternalEventing", "false"),
            ]);
        });
        builder.ConfigureTestServices(_ =>
        {
            Environment.SetEnvironmentVariable("MIGRATING", "1");
        });
    }
}
