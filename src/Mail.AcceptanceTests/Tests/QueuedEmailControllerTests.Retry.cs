// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Data;
using cCoder.Data.Models.Mail;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class QueuedEmailControllerTests
{
    [Fact]
    public async Task Retry_ShouldClearFailuresAndRetainQueuedEmail()
    {
        // Given
        SeededQueuedEmailContext seededContext = await SeedDatabase();

        QueuedEmail queuedEmail = await CreateQueuedEmailAsync(payload: new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject = Unique(prefix: "RetrySubject"),
            content = Unique(prefix: "RetryContent"),
            to = "recipient@example.test",
            cc = "",
            isBodyHtml = true,
            mailServerName = Unique(prefix: "RetryServer"),
        });

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            core.Set<EmailSendFailure>()
                .Add(entity: new EmailSendFailure
                {
                    EmailId = queuedEmail.Id,
                    AttemptedOn = DateTimeOffset.UtcNow,
                    FailureReason = "Transient failure",
                });

            await core.SaveChangesAsync();
        }

        // When
        using HttpResponseMessage response = await Client.PostAsync(
            requestUri: $"{BaseUrl}({queuedEmail.Id})/Retry",
            content: null);

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.NoContent);

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            core.Set<QueuedEmail>()
                .IgnoreQueryFilters()
                .Any(predicate: email => email.Id == queuedEmail.Id)
                .Should()
                .BeTrue();

            core.Set<EmailSendFailure>()
                .IgnoreQueryFilters()
                .Any(predicate: failure => failure.EmailId == queuedEmail.Id)
                .Should()
                .BeFalse();
        }

        await DeleteQueuedEmailAsync(id: queuedEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}