// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Providers;

public sealed partial class MailClientFactoryTests
{
    private readonly Mock<IMailClient> clientMock = new();
    private readonly Mock<IMailProviderService> providerServiceMock = new();

    [Fact]
    public void CreateMailClientShouldResolveAliasIgnoringCase()
    {
        providerServiceMock.Setup(x => x.GetMailClient("microsoftgraph"))
            .Returns(clientMock.Object);
        var factory = new MailClientFactory(providerServiceMock.Object);

        factory.CreateMailClient("microsoftgraph").Should().BeSameAs(clientMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("missing")]
    public void CreateMailClientShouldRejectUnknownProvider(string providerName)
    {
        providerServiceMock.Setup(x => x.GetMailClient(providerName))
            .Throws(new InvalidOperationException(
                $"No mail provider named '{providerName}' is registered."));
        var factory = new MailClientFactory(providerServiceMock.Object);

        Action action = () => factory.CreateMailClient(providerName);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"No mail provider named '{providerName}' is registered.");
    }

    [Fact]
    public async Task CreateMailClientAsyncShouldResolveReceiverProviderAsync()
    {
        Guid receiverId = Guid.NewGuid();
        clientMock.Setup(x => x.GetProviderNames()).Returns(["IMAP"]);
        providerServiceMock.Setup(x => x.GetMailClientAsync(receiverId, CancellationToken.None))
            .ReturnsAsync(clientMock.Object);
        var factory = new MailClientFactory(providerServiceMock.Object);

        IMailClient result = await factory.CreateMailClientAsync(receiverId);

        result.Should().BeSameAs(clientMock.Object);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005