# cCoder.Mail

`cCoder.Mail` contains the Mail domain for the cCoder platform. It provides mail-server configuration, queued email, sent email, mailbox receive support, event handling, and the background sender loop used by cCoder applications.

[View the latest main-branch code coverage report](https://ccoder-co-uk.github.io/cCoder.Mail/)

## Local Configuration

Each executable binds the complete configuration root to its own
`AppConfiguration`. The Web composition root registers `CoreData`, `Mail`,
`SecurityData`, `Security`, and `Eventing` side by side. The HostedServices
composition root registers `CoreData`, `Mail`, and `Eventing`.

Persistence belongs to the Data domains. `MailConfiguration` contains only
Mail behavior and provider availability; `CoreData` owns the database
connection, Data registration, and migrations. Likewise, `SecurityData` owns
the Security database and `Security` contains authentication behavior.

## Functionality

- Mail server management: configure application-owned SMTP settings, including host, port, SSL, sender, and credentials.
- Queued email management: create and inspect pending outbound emails.
- Sent email management: inspect emails that have been successfully dispatched.
- Mail provider abstraction: `cCoder.Mail.Providers` supplies the SPAL factory and the SMTP, POP3, IMAP, and Microsoft Graph implementations.
- Received email inspection: `ReceivedEmailController` can fetch Microsoft 365 mailbox messages without persisting them.
- Sender hosted service: checks the queue every minute and attempts SMTP delivery for pending messages.
- App lifecycle event handling: listens for app add, update, and delete events so mail-owned app data stays aligned.
- Manual test UI: `/tools/index.html` provides a lightweight CRUD surface for mail servers, queued mail, and sent mail, plus a received-mail tab for direct mailbox fetch testing.
- Operational health:
  - `Mail.Web` returns `OK` from `/Health`.
  - `Mail.HostedServices` returns `Healthy` from `/Health` and reports hosted services from `/`.

## Contents

- `src/cCoder.Mail`
  The main library package published to NuGet.
- `src/cCoder.Mail.Providers`
  The provider abstraction, factory, and provider implementations.
- `src/Mail.Web`
  The standalone web host for the Mail domain.
- `src/Mail.HostedServices`
  The hosted-services app for event listeners and the queued email sender.
- `src/cCoder.Mail.Tests`
  Unit tests for the domain.
- `src/Mail.AcceptanceTests`
  Acceptance tests for the standalone web host.
- `src/Mail.HostedServices.AcceptanceTests`
  Acceptance tests for the hosted-services app.
- `src/Mail.IntegrationTests`
  End-to-end tests that send queued mail through Microsoft Graph and receive it back through Microsoft Graph.

## Build

```powershell
dotnet build src/cCoder.Mail.slnx -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Mail.slnx -v minimal --no-build
```

The solution test run includes unit tests, app acceptance suites, and the mail delivery integration suite. Acceptance tests actively call the hosted HTTP surfaces, including health endpoints and the manual tools shell.

The end-to-end mail delivery test queues an email through `Mail.Web`, runs the sender orchestration, then calls the received-mail API until the same message is visible in the mailbox.

## Run Locally

```powershell
dotnet run --project src/Mail.Web/Mail.Web.csproj
dotnet run --project src/Mail.HostedServices/Mail.HostedServices.csproj
```

Useful `Mail.Web` endpoints:

- `/` redirects to `/tools/index.html`.
- `/tools/index.html` opens the manual domain tester.
- `/swagger` opens the API explorer.
- `/Health` returns `OK`.
- `/Api/Mail/ReceivedEmail/Receive` exposes the same receive endpoint on the Mail route.

Useful `Mail.HostedServices` endpoints:

- `/` returns a plain-text hosted-services report.
- `/Health` returns `Healthy`.

Leave secrets empty in `appsettings.json` and define the base required values as
user-level or machine-level environment variables:

- `CoreData__ConnectionString`
- `SecurityData__ConnectionString` (Web only)
- `Security__DecryptionKey` (Web only)
- `Eventing__ServiceBus__ConnectionString` when `Eventing__ProviderType` is
  `ServiceBus`

When Microsoft Graph is enabled, also define:

- `Mail__Providers__MicrosoftGraph__TenantId`
- `Mail__Providers__MicrosoftGraph__ClientId`
- `Mail__Providers__MicrosoftGraph__ClientSecret`

`CoreData__AdminConnectionString` and
`SecurityData__AdminConnectionString` are optional migration-only overrides. If
an admin connection is configured, startup migrations use it and normal runtime
operations continue to use the regular connection. If it is omitted, migrations
use the regular connection.

Library consumers register persistence and behavior explicitly at their own
composition root: call `AddData` before `AddMailWeb`, `AddMail`, or
`AddMailHostedServices`; Web hosts also call `AddSecurityData` before
`AddSecurityWeb`. An application that consumes `cCoder.Core` should use Core's
composite API instead; Core deliberately composes its configured child domains
recursively.

## Provider Configuration

Provider availability is expressed by entries in `Mail:Providers`. Omit a
provider to make it unavailable; there is no separate `Enabled` flag.

```json
{
  "Mail": {
    "Providers": {
      "Smtp": {},
      "Pop3": {},
      "Imap": {},
      "MicrosoftGraph": {
        "TenantId": "",
        "ClientId": "",
        "ClientSecret": ""
      }
    }
  }
}
```

`MailSender.ProviderName` and `MailReceiver.ProviderName` select the client at
runtime. Their database records contain mailbox-specific host, port, user, and
password values. Platform-wide Graph application credentials remain in
configuration. Every provider implements `IMailClient`; invoking an unsupported
send or receive operation throws `UnsupportedMailClientOperationException`.

For SMTP delivery, `Mail:Providers:Smtp` must be present in configuration, the
application must have a valid `MailSender` row whose `ProviderName` is `Smtp`,
and that row must reference the required server/credential data. A queued email
is sent by the Mail hosted-services process, so configuring a sender row without
running that process does not dispatch queued messages. Microsoft Graph follows
the same database-row selection model, but its platform-wide tenant, client, and
client-secret values come from `Mail:Providers:MicrosoftGraph`.

## Mail Delivery Integration

The real send-and-receive integration test requires these variables on the runner:

- `CoreData__ConnectionString`
- `SecurityData__ConnectionString`
- `Security__DecryptionKey`
- `Mail__Providers__MicrosoftGraph__TenantId`
- `Mail__Providers__MicrosoftGraph__ClientId`
- `Mail__Providers__MicrosoftGraph__ClientSecret`

The test creates disposable integration databases by appending
`-acceptance-{guid}` to the configured Mail and Security database names.
It sends from `SendUser` to `ReceiveUser`, polls every 10 seconds for up to
120 seconds, and retrieves at most 50 messages.
The Graph application registration must have `Mail.Send` and `Mail.Read` application permissions with admin consent applied.

## Package

The NuGet packages produced by this repository are:

- `cCoder.Mail`
- `cCoder.Mail.Providers`

## Publishing

GitHub Actions is configured to publish the main package using NuGet trusted publishing.

Before the first publish, configure a trusted publishing policy on nuget.org for:

- Repository owner: `ccoder-co-uk`
- Repository: `cCoder.Mail`
- Workflow file: `publish.yml`

The workflow also expects a `NUGET_USER` repository secret containing the nuget.org profile name used during trusted publishing login.