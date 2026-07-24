// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Exposures.MailClients;
using cCoder.Mail.Models;
using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class MailSenderFactoryTests
{
    [Fact]
    public void ShouldReturnDefaultSenderWhenProviderNameIsMissing()
    {
        // When
        IMailSenderProvider actualProvider = mailSenderFactory.GetSender(providerName: null);

        // Then

        actualProvider.Should()
            .BeSameAs(expected: smtpSenderProviderMock.Object);
    }

    [Fact]
    public void ShouldReturnMicrosoftGraphSenderWhenProviderAliasIsGraphHost()
    {
        // When
        IMailSenderProvider actualProvider = mailSenderFactory.GetSender(providerName: "graph.microsoft.com");

        // Then

        actualProvider.Should()
            .BeSameAs(expected: graphSenderProviderMock.Object);
    }

    [Fact]
    public void ShouldReturnDefaultSenderWhenProviderNameIsAnUnmappedSmtpHost()
    {
        // When
        IMailSenderProvider actualProvider = mailSenderFactory.GetSender(providerName: "smtp.office365.com");

        // Then

        actualProvider.Should()
            .BeSameAs(expected: smtpSenderProviderMock.Object);
    }

    [Fact]
    public void ShouldThrowWhenSenderProviderNameIsUnknown()
    {
        // When
        Action action = () => mailSenderFactory.GetSender(providerName: "Unknown");

        // Then

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "No mail sender provider named 'Unknown' is registered.");
    }

    [Fact]
    public void ShouldReturnCustomMicrosoftGraphSenderWhenConfigured()
    {
        // Given
        mailConfiguration.AddMicrosoftGraphSender(name: "CorporateGraph");

        // When
        IMailSenderProvider actualProvider = mailSenderFactory.GetSender(providerName: "CorporateGraph");

        // Then

        actualProvider.Should()
            .BeSameAs(expected: graphSenderProviderMock.Object);
    }
}