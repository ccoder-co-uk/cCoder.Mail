using System.Net;
using cCoder.Data;
using cCoder.Data.Models.Mail;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


using Web.AcceptanceTests.Infrastructure;
namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class MailServerControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetMailServerCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfMailServers()
    {
        // Given

        // When
        IReadOnlyList<MailServer> actualMailServers = await GetMailServersAsync(1);

        // Then
        actualMailServers.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsMailServerById()
    {
        // Given
        SeededMailServerContext seededContext = await SeedDatabase();
        string name = Unique("MailServer");
        MailServer expectedMailServer = await CreateMailServerAsync(new
        {
            appId = seededContext.AppId,
            name,
            user = "acceptance",
            password = "password",
            host = "smtp.acceptance.local",
            fromEmail = "acceptance@example.com",
            port = 25,
            enableSSL = false,
        });
        MailServer actualMailServer;

        // When
        actualMailServer = await GetMailServerAsync(expectedMailServer.Id);

        // Then
        actualMailServer.Should().NotBeNull();
        actualMailServer.Id.Should().Be(expectedMailServer.Id);
        actualMailServer.Name.Should().Be(name);

        await DeleteMailServerAsync(expectedMailServer.Id);
        await Teardown(seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededMailServerContext seededContext = await SeedDatabase("mailserver_create", "mailserver_update", "mailserver_delete");

        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        MailServer hiddenMailServer = await core.AddMailServerAsync(new MailServer
        {
            AppId = seededContext.AppId,
            Name = Unique("HiddenMailServer"),
            User = "acceptance",
            Password = "password",
            Host = "smtp.acceptance.local",
            FromEmail = "acceptance@example.com",
            Port = 25,
            EnableSSL = false,
        });

        int actualStatusCode = await GetMailServerStatusCodeAsync(hiddenMailServer.Id);

        actualStatusCode.Should().Be((int)HttpStatusCode.NotFound);

        await Teardown(seededContext);
    }
}






