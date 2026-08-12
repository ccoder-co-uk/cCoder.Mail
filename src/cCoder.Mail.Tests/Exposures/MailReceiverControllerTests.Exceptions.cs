// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public partial class MailReceiverControllerTests
{
    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void ShouldReturnServerErrorWhenGetFails(Exception exception, int expectedStatusCode)
    {
        mailReceiverManagerMock.Setup(expression: service => service.GetAllMailReceiver(false))
            .Throws(exception: exception);

        IActionResult result = controller.Get(key: Guid.Empty);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Fact]
    public void ShouldReturnServerErrorWhenGetMetadataFails()
    {
        controller.ControllerContext = new ControllerContext();

        IActionResult result = controller.GetMetadata();

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(500);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void ShouldReturnServerErrorWhenGetAllFails(Exception exception, int expectedStatusCode)
    {
        mailReceiverManagerMock.Setup(expression: service => service.GetAllMailReceiver(false))
            .Throws(exception: exception);

        IActionResult result = controller.GetAll();

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPostFailsAsync(Exception exception, int expectedStatusCode)
    {
        MailReceiver item = new();
        mailReceiverManagerMock.Setup(expression: service => service.AddMailReceiverAsync(item))
            .Throws(exception: exception);

        IActionResult result = await controller.Post(newMailReceiver: item);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPutFailsAsync(Exception exception, int expectedStatusCode)
    {
        MailReceiver item = new();
        mailReceiverManagerMock.Setup(expression: service => service.UpdateMailReceiverAsync(item))
            .Throws(exception: exception);

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailReceiver: item);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPatchFailsAsync(Exception exception, int expectedStatusCode)
    {
        mailReceiverManagerMock.Setup(expression: service => service.GetMailReceiver(iMailReceiverId: Guid.Empty))
            .Throws(exception: exception);

        IActionResult result = await controller.Put(
            key: Guid.Empty,
            updatedMailReceiver: new Delta<MailReceiver>());

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenDeleteFailsAsync(Exception exception, int expectedStatusCode)
    {
        mailReceiverManagerMock.Setup(expression: service => service.DeleteAsync(iMailReceiverId: Guid.Empty))
            .Throws(exception: exception);

        IActionResult result = await controller.Delete(key: Guid.Empty);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005