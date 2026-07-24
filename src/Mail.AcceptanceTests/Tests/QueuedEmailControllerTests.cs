// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.Mail;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class QueuedEmailControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/QueuedEmail";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededQueuedEmailContext(int AppId, Guid RoleId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededQueuedEmailContext> SeedDatabase()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "queuedemail")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        Role role = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "AcceptanceRole"),
            Description = "Acceptance role",
            Privs = "app_admin,queuedemail_create,queuedemail_update,queuedemail_delete,queuedemail_read",
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = role.Id, UserId = "Guest" });

        return new SeededQueuedEmailContext(AppId: app.Id, RoleId: role.Id);
    }

    private async Task<QueuedEmail> CreateQueuedEmailAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<QueuedEmail>(json: content, options: JsonOptions)
            ?? throw new InvalidOperationException(message: "Expected queued email payload.");
    }

    private async Task<int> UpdateQueuedEmailAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({id})", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchQueuedEmailAsync(int id, object payload)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> DeleteQueuedEmailAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<QueuedEmail> GetQueuedEmailAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<QueuedEmail>(json: content, options: JsonOptions)
            ?? throw new InvalidOperationException(message: "Expected queued email payload.");
    }

    private async Task Teardown(SeededQueuedEmailContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        EmailSendFailure[] failures = core.Set<EmailSendFailure>()
            .IgnoreQueryFilters()
            .Where(predicate: failure => failure.Email.AppId == seededContext.AppId)
            .ToArray();

        await core.DeleteAllAsync(sendFailures: failures);

        QueuedEmail[] queuedEmails = core.Set<QueuedEmail>()
            .IgnoreQueryFilters()
            .Where(predicate: email => email.AppId == seededContext.AppId)
            .ToArray();

        await core.DeleteAllAsync(queuedEmails: queuedEmails);

        UserRole[] userRoles = core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => userRole.RoleId == seededContext.RoleId)
            .ToArray();

        await core.DeleteAllAsync(userRoles: userRoles);

        Role role = core.Set<Role>()
            .IgnoreQueryFilters()
            .Single(predicate: found => found.Id == seededContext.RoleId);

        await core.DeleteAsync(role: role);

        App app = core.Set<App>()
            .IgnoreQueryFilters()
            .Single(predicate: found => found.Id == seededContext.AppId);

        await core.DeleteAsync(app: app);
    }

    private async Task<int> GetQueuedEmailCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<QueuedEmail>> GetQueuedEmailsAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<QueuedEmail>>(json: content, options: JsonOptions)?.Value
            ?? throw new InvalidOperationException(message: "Expected queued email OData payload.");
    }

    private async Task<int> GetQueuedEmailStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}