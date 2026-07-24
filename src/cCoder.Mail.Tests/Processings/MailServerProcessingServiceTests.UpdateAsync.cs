// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();

        mailServerServiceMock.Setup(expression: x => x.UpdateAsync(mailServer: entity))
            .ReturnsAsync(value: entity);

        // When
        MailServer result = await mailServerProcessingService.UpdateAsync(entity: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        mailServerServiceMock.Verify(expression: x => x.UpdateAsync(mailServer: entity), times: Times.Once);
        mailServerServiceMock.VerifyNoOtherCalls();
    }

}