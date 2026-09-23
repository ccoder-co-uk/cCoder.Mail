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
    private readonly Mock<IMailReceiverProviderService> receiverProviderServiceMock = new();

    [Fact]
    public void CreateMailClientShouldResolveAliasIgnoringCase()
    {
        clientMock.Setup(x => x.GetProviderNames())
            .Returns(["MicrosoftGraph"]);
        MailClientFactory factory = CreateFactory();

        factory.CreateMailClient("microsoftgraph").Should().BeSameAs(clientMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("missing")]
    public void CreateMailClientShouldRejectUnknownProvider(string providerName)
    {
        clientMock.Setup(x => x.GetProviderNames()).Returns(["SMTP"]);
        MailClientFactory factory = CreateFactory();

        Action action = () => factory.CreateMailClient(providerName);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"No mail provider named '{providerName}' is registered.");
    }

    [Fact]
    public async Task CreateMailClientAsyncShouldResolveReceiverProviderAsync()
    {
        Guid receiverId = Guid.NewGuid();
        clientMock.Setup(x => x.GetProviderNames()).Returns(["IMAP"]);
        receiverProviderServiceMock
            .Setup(x => x.RetrieveMailReceiverProviderNameAsync(
                receiverId,
                CancellationToken.None))
            .ReturnsAsync("IMAP");
        MailClientFactory factory = CreateFactory();

        IMailClient result = await factory.CreateMailClientAsync(receiverId);

        result.Should().BeSameAs(clientMock.Object);
    }

    private MailClientFactory CreateFactory() =>
        new(
            mailClients: [clientMock.Object],
            mailReceiverProviderService: receiverProviderServiceMock.Object);

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005