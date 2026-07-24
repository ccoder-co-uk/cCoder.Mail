// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        string failureReason = Unique(prefix: "FailureReason");

        QueuedEmail queuedEmail = await CreateQueuedEmailAsync(payload: new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject = Unique(prefix: "Subject"),
            content = Unique(prefix: "Content"),
            to = "recipient@example.test",
            cc = "",
            isBodyHtml = true,
            mailServerName = Unique(prefix: "Server"),
        });

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            core.Set<EmailSendFailure>()
                .Add(entity: new EmailSendFailure
                {
                    EmailId = queuedEmail.Id,
                    AttemptedOn = DateTimeOffset.UtcNow,
                    FailureReason = failureReason,
                });

            await core.SaveChangesAsync();
        }

        // When

        using HttpResponseMessage response = await Client.GetAsync(
requestUri: $"{BaseUrl}?$filter=Id eq {queuedEmail.Id}&$expand=FailedSends");

        string content = await response.Content.ReadAsStringAsync();

        // Then

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        content.Should()
            .Contain(expected: "FailedSends");

        content.Should()
            .Contain(expected: failureReason);

        await DeleteQueuedEmailAsync(id: queuedEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}