# cCoder.Mail

`cCoder.Mail` contains the Mail domain for the cCoder platform. It provides mail-server configuration, queued email, sent email, mailbox receive support, event handling, and the background sender loop used by cCoder applications.

## Local Configuration

Configuration binds directly into `MailConfiguration`. Leave secrets empty in
appsettings and define `Mail__ConnectionString` plus any configured provider
secrets as user-level or machine-level environment variables. Providers are
keyed by name, so the Microsoft Graph secret is
`Mail__Providers__MicrosoftGraph__ClientSecret`. Restart Visual Studio,
select the Web and HostedServices startup projects, and press F5. No
configuration conversion step is required.

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

The runnable apps bind the structured settings directly. Their required secrets
are:

- `Mail__ConnectionString`
- `Security__ConnectionString` (Web only)
- `Security__DecryptionKey` (Web only)
- `Mail__Providers__MicrosoftGraph__TenantId`
- `Mail__Providers__MicrosoftGraph__ClientId`
- `Mail__Providers__MicrosoftGraph__ClientSecret`

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

## Mail Delivery Integration

The real send-and-receive integration test requires these variables on the runner:

- `Mail__ConnectionString`
- `Security__ConnectionString`
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
