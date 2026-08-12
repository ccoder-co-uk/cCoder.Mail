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

public partial class MailSenderControllerTests
{
    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void ShouldReturnServerErrorWhenGetFails(Exception exception, int expectedStatusCode)
    {
        mailSenderManagerMock.Setup(expression: service => service.GetAllMailSender(false))
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
        mailSenderManagerMock.Setup(expression: service => service.GetAllMailSender(false))
            .Throws(exception: exception);

        IActionResult result = controller.GetAll();

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPostFailsAsync(Exception exception, int expectedStatusCode)
    {
        MailSender item = new();
        mailSenderManagerMock.Setup(expression: service => service.AddMailSenderAsync(item))
            .Throws(exception: exception);

        IActionResult result = await controller.Post(newMailSender: item);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPutFailsAsync(Exception exception, int expectedStatusCode)
    {
        MailSender item = new();
        mailSenderManagerMock.Setup(expression: service => service.UpdateMailSenderAsync(item))
            .Throws(exception: exception);

        IActionResult result = await controller.Put(key: Guid.Empty, updatedMailSender: item);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPatchFailsAsync(Exception exception, int expectedStatusCode)
    {
        mailSenderManagerMock.Setup(expression: service => service.GetMailSender(iMailSenderId: Guid.Empty))
            .Throws(exception: exception);

        IActionResult result = await controller.Put(
            key: Guid.Empty,
            updatedMailSender: new Delta<MailSender>());

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenDeleteFailsAsync(Exception exception, int expectedStatusCode)
    {
        mailSenderManagerMock.Setup(expression: service => service.DeleteAsync(iMailSenderId: Guid.Empty))
            .Throws(exception: exception);

        IActionResult result = await controller.Delete(key: Guid.Empty);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005