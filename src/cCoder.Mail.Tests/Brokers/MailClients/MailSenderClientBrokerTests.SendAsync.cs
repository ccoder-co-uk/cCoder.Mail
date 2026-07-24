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

        mailSenderFactoryMock
            .Setup(expression: factory => factory.GetSender(providerName: null))
            .Returns(value: mailSenderProviderMock.Object);

        mailSenderProviderMock
            .Setup(expression: provider => provider.SendAsync(email: email, cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        await mailSenderClientBroker.SendAsync(email: email, cancellationToken: cancellationToken);

        // Then
        mailSenderFactoryMock.Verify(expression: factory => factory.GetSender(providerName: null), times: Times.Once);
        mailSenderProviderMock.Verify(expression: provider => provider.SendAsync(email: email, cancellationToken: cancellationToken), times: Times.Once);
        mailSenderFactoryMock.VerifyNoOtherCalls();
        mailSenderProviderMock.VerifyNoOtherCalls();
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

        mailSenderFactoryMock
            .Setup(expression: factory => factory.GetSender(providerName: "MicrosoftGraph"))
            .Returns(value: mailSenderProviderMock.Object);

        mailSenderProviderMock
            .Setup(expression: provider => provider.SendAsync(email: email, cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        await mailSenderClientBroker.SendAsync(email: email, cancellationToken: cancellationToken);

        // Then
        mailSenderFactoryMock.Verify(expression: factory => factory.GetSender(providerName: "MicrosoftGraph"), times: Times.Once);
        mailSenderProviderMock.Verify(expression: provider => provider.SendAsync(email: email, cancellationToken: cancellationToken), times: Times.Once);
        mailSenderFactoryMock.VerifyNoOtherCalls();
        mailSenderProviderMock.VerifyNoOtherCalls();
    }
}