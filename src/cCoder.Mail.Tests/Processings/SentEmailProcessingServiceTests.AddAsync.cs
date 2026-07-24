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

public partial class SentEmailProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail();

        sentEmailServiceMock.Setup(expression: x => x.AddAsync(sentEmail: sentEmail))
            .ReturnsAsync(value: sentEmail);

        // When
        SentEmail result = await sentEmailProcessingService.AddAsync(entity: sentEmail);

        // Then
        Assert.Same(expected: sentEmail, actual: result);
        sentEmailServiceMock.Verify(expression: x => x.AddAsync(sentEmail: sentEmail), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail();

        sentEmailServiceMock
            .Setup(expression: x => x.AddAsync(sentEmail: sentEmail))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
            await sentEmailProcessingService.AddAsync(entity: sentEmail)
        );

        // Then
    }

}