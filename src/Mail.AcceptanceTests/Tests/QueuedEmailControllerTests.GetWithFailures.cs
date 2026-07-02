using System.Net;
using cCoder.Data;
using cCoder.Data.Models.Mail;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class QueuedEmailControllerTests
{
    [Fact]
    public async Task Get_WithSendFailures_ReturnsExpandedFailures()
    {
        // Given
        SeededQueuedEmailContext seededContext = await SeedDatabase();
        string failureReason = Unique("FailureReason");
        QueuedEmail queuedEmail = await CreateQueuedEmailAsync(new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject = Unique("Subject"),
            content = Unique("Content"),
            to = "recipient@example.test",
            cc = "",
            isBodyHtml = true,
            mailServerName = Unique("Server"),
        });

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            core.Set<EmailSendFailure>().Add(new EmailSendFailure
            {
                EmailId = queuedEmail.Id,
                AttemptedOn = DateTimeOffset.UtcNow,
                FailureReason = failureReason,
            });

            await core.SaveChangesAsync();
        }

        // When
        using HttpResponseMessage response = await Client.GetAsync(
            $"{BaseUrl}?$filter=Id eq {queuedEmail.Id}&$expand=FailedSends");
        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        content.Should().Contain("FailedSends");
        content.Should().Contain(failureReason);

        await DeleteQueuedEmailAsync(queuedEmail.Id);
        await Teardown(seededContext);
    }
}
