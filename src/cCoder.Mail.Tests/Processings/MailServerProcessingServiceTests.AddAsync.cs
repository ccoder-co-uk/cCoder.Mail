// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer();

        mailServerServiceMock.Setup(expression: x => x.AddAsync(mailServer: mailServer))
            .ReturnsAsync(value: mailServer);

        // When
        MailServer result = await mailServerProcessingService.AddAsync(entity: mailServer);

        // Then
        Assert.Same(expected: mailServer, actual: result);
        mailServerServiceMock.Verify(expression: x => x.AddAsync(mailServer: mailServer), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer();

        mailServerServiceMock
            .Setup(expression: x => x.AddAsync(mailServer: mailServer))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
            await mailServerProcessingService.AddAsync(entity: mailServer)
        );

        // Then
    }

}