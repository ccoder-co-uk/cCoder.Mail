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

        sentEmailServiceMock.Setup(expression: x => x.AddSentEmailAsync(newSentEmail: sentEmail))
            .ReturnsAsync(value: sentEmail);

        // When
        SentEmail result = await sentEmailProcessingService.AddSentEmailAsync(newSentEmail: sentEmail);

        // Then
        Assert.Same(expected: sentEmail, actual: result);
        sentEmailServiceMock.Verify(expression: x => x.AddSentEmailAsync(newSentEmail: sentEmail), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail();

        sentEmailServiceMock
            .Setup(expression: x => x.AddSentEmailAsync(newSentEmail: sentEmail))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.Mail.Models.Exceptions.MailServiceException>(testCode: async () =>
            await sentEmailProcessingService.AddSentEmailAsync(newSentEmail: sentEmail)
        );

        // Then
    }

}