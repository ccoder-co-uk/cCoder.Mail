// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Providers.Services.Foundations;
using cCoder.Mail.Providers.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Providers;

public sealed partial class MailProviderServiceTests
{
    public static TheoryData<Exception, Type> ProviderExceptionMappings =>
        new()
        {
            {
                new MailValidationException(innerException: new Exception()),
                typeof(MailValidationException)
            },
            {
                new MailDependencyException(innerException: new Exception()),
                typeof(MailDependencyException)
            },
            {
                new ArgumentException(message: "invalid"),
                typeof(MailValidationException)
            },
            {
                new InvalidOperationException(message: "invalid state"),
                typeof(InvalidOperationException)
            },
            {
                new Exception(message: "failed"),
                typeof(MailServiceException)
            }
        };

    [Theory]
    [MemberData(nameof(ProviderExceptionMappings))]
    public void GetMailClientShouldMapException(
        Exception exception,
        Type expectedType)
    {
        var providerServiceMock = new Mock<IMailProviderService>(
            behavior: MockBehavior.Strict);
        var receiverServiceMock = new Mock<IMailReceiverProviderService>(
            behavior: MockBehavior.Strict);

        providerServiceMock
            .Setup(expression: service => service.GetMailClient(
                It.IsAny<string>()))
            .Throws(exception: exception);

        var service = new MailProviderOrchestrationService(
            mailProviderService: providerServiceMock.Object,
            mailReceiverProviderService: receiverServiceMock.Object);

        Action action = () => service.GetMailClient(providerName: "SMTP");

        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ProviderExceptionMappings))]
    public async Task GetMailClientAsyncShouldMapExceptionAsync(
        Exception exception,
        Type expectedType)
    {
        var providerServiceMock = new Mock<IMailProviderService>(
            behavior: MockBehavior.Strict);
        var receiverServiceMock = new Mock<IMailReceiverProviderService>(
            behavior: MockBehavior.Strict);

        receiverServiceMock
            .Setup(expression: service => service
                .RetrieveMailReceiverProviderNameAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception: exception);

        var service = new MailProviderOrchestrationService(
            mailProviderService: providerServiceMock.Object,
            mailReceiverProviderService: receiverServiceMock.Object);

        Func<Task> action = async () =>
            await service.GetMailClientAsync(mailReceiverId: Guid.NewGuid());

        (await action.Should().ThrowAsync<Exception>())
            .Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005