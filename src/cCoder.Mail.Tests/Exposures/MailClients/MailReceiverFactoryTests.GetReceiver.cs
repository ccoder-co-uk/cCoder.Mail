// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        IMailReceiverProvider actualProvider = mailReceiverFactory.GetReceiver(providerName: null);

        // Then

        actualProvider.Should()
            .BeSameAs(expected: graphReceiverProviderMock.Object);
    }

    [Fact]
    public void ShouldReturnPop3ReceiverWhenProviderNameIsPop3()
    {
        // When
        IMailReceiverProvider actualProvider = mailReceiverFactory.GetReceiver(providerName: "Pop3");

        // Then

        actualProvider.Should()
            .BeSameAs(expected: pop3ReceiverProviderMock.Object);
    }

    [Fact]
    public void ShouldReturnMicrosoftGraphReceiverWhenProviderAliasIsGraphHost()
    {
        // When
        IMailReceiverProvider actualProvider = mailReceiverFactory.GetReceiver(providerName: "graph.microsoft.com");

        // Then

        actualProvider.Should()
            .BeSameAs(expected: graphReceiverProviderMock.Object);
    }

    [Fact]
    public void ShouldThrowWhenReceiverProviderNameIsUnknown()
    {
        // When
        Action action = () => mailReceiverFactory.GetReceiver(providerName: "Unknown");

        // Then

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "No mail receiver provider named 'Unknown' is registered.");
    }
}