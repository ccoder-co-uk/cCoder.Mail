// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        var id = entity.Id;

        sentEmailServiceMock.Setup(expression: x => x.DeleteAsync(iSentEmailId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await sentEmailProcessingService.DeleteAsync(sentEmailId: id);

        // Then
        sentEmailServiceMock.Verify(expression: x => x.DeleteAsync(iSentEmailId: id), times: Times.Once);
        sentEmailServiceMock.VerifyNoOtherCalls();
    }

}