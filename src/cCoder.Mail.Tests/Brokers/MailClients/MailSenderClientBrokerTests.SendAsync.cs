// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailSenderClientBrokerTests
{
    [Fact]
    public async Task ShouldDelegateToDefaultSenderWhenSendAsync()
    {
        // Given
        QueuedEmail email = new() { Subject = "Send" };
        CancellationToken cancellationToken = new();

        mailClientFactoryMock
            .Setup(
                expression: factory =>
                    factory.CreateMailClient(
                        providerName: null))
            .Returns(value: mailClientMock.Object);

        mailClientMock
            .Setup(expression: provider => provider.SendAsync(email: email, cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        await mailSenderClientBroker.SendAsync(email: email, cancellationToken: cancellationToken);

        // Then

        mailClientFactoryMock.Verify(
            expression: factory =>
                factory.CreateMailClient(
                    providerName: null),
            times: Times.Once);

        mailClientMock.Verify(expression: provider => provider.SendAsync(email: email, cancellationToken: cancellationToken), times: Times.Once);
        mailClientFactoryMock.VerifyNoOtherCalls();
        mailClientMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToProviderSelectedByMailSenderProviderNameWhenSendAsync()
    {
        // Given
        QueuedEmail email = new()
        {
            Subject = "Send",
            MailSender = new MailSender
            {
                Name = "Graph",
                ProviderName = "MicrosoftGraph",
            },
        };

        CancellationToken cancellationToken = new();

        mailClientFactoryMock
            .Setup(
                expression: factory =>
                    factory.CreateMailClient(
                        providerName:
                            "MicrosoftGraph"))
            .Returns(value: mailClientMock.Object);

        mailClientMock
            .Setup(expression: provider => provider.SendAsync(email: email, cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        await mailSenderClientBroker.SendAsync(email: email, cancellationToken: cancellationToken);

        // Then

        mailClientFactoryMock.Verify(
            expression: factory =>
                factory.CreateMailClient(
                    providerName:
                        "MicrosoftGraph"),
            times: Times.Once);

        mailClientMock.Verify(expression: provider => provider.SendAsync(email: email, cancellationToken: cancellationToken), times: Times.Once);
        mailClientFactoryMock.VerifyNoOtherCalls();
        mailClientMock.VerifyNoOtherCalls();
    }
}