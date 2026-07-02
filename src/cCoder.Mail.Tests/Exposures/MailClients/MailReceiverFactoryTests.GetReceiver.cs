using cCoder.Mail.Exposures.MailClients;
using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class MailReceiverFactoryTests
{
    [Fact]
    public void ShouldReturnDefaultReceiverWhenProviderNameIsMissing()
    {
        // When
        IMailReceiverProvider actualProvider = mailReceiverFactory.GetReceiver(null);

        // Then
        actualProvider.Should().BeSameAs(graphReceiverProviderMock.Object);
    }

    [Fact]
    public void ShouldReturnPop3ReceiverWhenProviderNameIsPop3()
    {
        // When
        IMailReceiverProvider actualProvider = mailReceiverFactory.GetReceiver("Pop3");

        // Then
        actualProvider.Should().BeSameAs(pop3ReceiverProviderMock.Object);
    }

    [Fact]
    public void ShouldReturnMicrosoftGraphReceiverWhenProviderAliasIsGraphHost()
    {
        // When
        IMailReceiverProvider actualProvider = mailReceiverFactory.GetReceiver("graph.microsoft.com");

        // Then
        actualProvider.Should().BeSameAs(graphReceiverProviderMock.Object);
    }

    [Fact]
    public void ShouldThrowWhenReceiverProviderNameIsUnknown()
    {
        // When
        Action action = () => mailReceiverFactory.GetReceiver("Unknown");

        // Then
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("No mail receiver provider named 'Unknown' is registered.");
    }
}
