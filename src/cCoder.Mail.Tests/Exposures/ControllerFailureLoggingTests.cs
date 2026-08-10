// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures;
using cCoder.Mail.Exposures.Controllers;
using cCoder.Mail.Providers.Models.Exceptions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed partial class ControllerFailureLoggingTests
{
    [Fact]
    public async Task Controllers_ShouldLogMappedFailures()
    {
        // Given
        Exception[] exceptions =
        [
            new MailValidationException(innerException: new Exception()),
            new SecurityException(),
            new Exception(),
        ];

        // When
        foreach (Exception exception in exceptions)
        {
            await ExerciseSentEmailControllerAsync(exception: exception);
            await ExerciseReceivedEmailControllerAsync(exception: exception);
        }

        // Then
        // Logging is verified by each exercised controller helper.
    }

    private static async Task ExerciseSentEmailControllerAsync(Exception exception)
    {
        Mock<ISentEmailManager> serviceMock = new();
        Mock<ILoggingBroker> loggingBrokerMock = new();

        serviceMock
            .Setup(expression: service => service.DeleteAsync(
                iSentEmailId: It.IsAny<int>()))
            .Throws(exception: exception);

        serviceMock
            .Setup(expression: service => service.GetAllSentEmail(
                ignoreFilters: It.IsAny<bool>()))
            .Throws(exception: exception);

        serviceMock
            .Setup(expression: service => service.AddSentEmailAsync(
                newSentEmail: It.IsAny<SentEmail>()))
            .Throws(exception: exception);

        serviceMock
            .Setup(expression: service => service.UpdateSentEmailAsync(
                updatedSentEmail: It.IsAny<SentEmail>()))
            .Throws(exception: exception);

        SentEmailController controller = new(
            service: serviceMock.Object,
            loggingBroker: loggingBrokerMock.Object);

        await controller.Delete(key: 1);
        controller.Get(key: 1);
        controller.GetAll();
        await controller.Post(newSentEmail: new SentEmail());
        await controller.Put(key: 1, updatedSentEmail: new SentEmail());

        loggingBrokerMock.Verify(
            expression: broker => broker.LogError(
                exception: exception,
                message: "Controller request failed.",
                args: It.IsAny<object[]>()),
            times: Times.Exactly(callCount: 5));
    }

    private static async Task ExerciseReceivedEmailControllerAsync(Exception exception)
    {
        Mock<IReceivedEmailManager> serviceMock = new();
        Mock<ILoggingBroker> loggingBrokerMock = new();

        serviceMock
            .Setup(expression: service => service.DeleteAsync(
                iReceivedEmailId: It.IsAny<int>()))
            .Throws(exception: exception);

        serviceMock
            .Setup(expression: service => service.GetAllReceivedEmail(
                ignoreFilters: It.IsAny<bool>()))
            .Throws(exception: exception);

        serviceMock
            .Setup(expression: service => service.AddReceivedEmailAsync(
                newReceivedEmail: It.IsAny<ReceivedEmail>()))
            .Throws(exception: exception);

        serviceMock
            .Setup(expression: service => service.UpdateReceivedEmailAsync(
                updatedReceivedEmail: It.IsAny<ReceivedEmail>()))
            .Throws(exception: exception);

        ReceivedEmailController controller = new(
            service: serviceMock.Object,
            loggingBroker: loggingBrokerMock.Object);

        await controller.Delete(key: 1);
        controller.Get(key: 1);
        controller.GetAll();
        await controller.Post(newReceivedEmail: new ReceivedEmail());
        await controller.Put(key: 1, updatedReceivedEmail: new ReceivedEmail());

        loggingBrokerMock.Verify(
            expression: broker => broker.LogError(
                exception: exception,
                message: "Controller request failed.",
                args: It.IsAny<object[]>()),
            times: Times.Exactly(callCount: 5));
    }
}