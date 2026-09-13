// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Exposures.MailClients;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Brokers.MailClients;

public sealed partial class MailClientRegistryBrokerTests
{
    [Fact]
    public void SelectMailClientShouldResolveAliasIgnoringCase()
    {
        var clientMock = new Mock<IMailClient>();
        clientMock.Setup(x => x.GetProviderNames()).Returns(["Graph", "MicrosoftGraph"]);
        var broker = new MailClientRegistryBroker([clientMock.Object]);

        IMailClient result = broker.SelectMailClient("microsoftgraph");

        result.Should().BeSameAs(clientMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("missing")]
    public void SelectMailClientShouldRejectUnknownProvider(string providerName)
    {
        var clientMock = new Mock<IMailClient>();
        clientMock.Setup(x => x.GetProviderNames()).Returns(["Graph"]);
        var broker = new MailClientRegistryBroker([clientMock.Object]);

        Action action = () => broker.SelectMailClient(providerName);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"No mail provider named '{providerName}' is registered.");
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005