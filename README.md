# cCoder.Mail

`cCoder.Mail` contains the Mail domain for the cCoder platform. It provides mail-server configuration, queued email, sent email, mailbox receive support, event handling, and the background sender loop used by cCoder applications.

## Local Configuration

Configuration binds directly into `MailConfiguration`. Leave secrets empty in
appsettings and define `Mail__ConnectionString` plus any configured provider
secrets, such as `Mail__MicrosoftGraph__ClientSecret`, as user-level or
machine-level environment variables. Restart Visual Studio, select the Web and
HostedServices startup projects, and press F5. No configuration conversion step
is required.

## Functionality

- Mail server management: configure application-owned SMTP settings, including host, port, SSL, sender, and credentials.
- Queued email management: create and inspect pending outbound emails.
- Sent email management: inspect emails that have been successfully dispatched.
- Mail provider abstraction: sender and receiver factories route mail work to named providers, with SMTP, POP3, and Microsoft Graph providers registered by default.
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
- `Mail__MicrosoftGraph__TenantId`
- `Mail__MicrosoftGraph__ClientId`
- `Mail__MicrosoftGraph__ClientSecret`
- `Mail__MicrosoftGraph__SendUser`
- `Mail__MicrosoftGraph__ReceiveUser`

Provider-specific POP3 and IMAP secrets use the matching structured paths, such
as `Mail__Pop3__Password` and `Mail__Imap__Password`.

## Provider Configuration

Library consumers can configure sender and receiver providers when adding Mail services:

```csharp
services.AddMail(mailConfig =>
{
    mailConfig.AddMicrosoftGraphSender(graphConfig =>
    {
        graphConfig.TenantId = tenantId;
        graphConfig.ClientId = clientId;
        graphConfig.ClientSecret = clientSecret;
        graphConfig.SendUser = sendUser;
        graphConfig.ReceiveUser = receiveUser;
    });

    mailConfig.AddMicrosoftGraphReceiver();
});
```

Registered sender providers are resolved by provider name. SMTP remains the default sender, so existing `MailServer.Host` values such as `smtp.office365.com` continue to use the SMTP provider. Microsoft Graph can be selected with provider names or aliases such as `MicrosoftGraph`, `graph.microsoft.com`, `https://graph.microsoft.com`, or `microsoft-graph`.

Registered receiver providers are resolved by provider name. Microsoft Graph is the default receiver for direct mailbox receive calls, while POP3 remains available through the `Pop3` provider.

Custom providers can be added by registering an implementation of `IMailSenderProvider` or `IMailReceiverProvider`, then mapping the public provider name with `AddSenderProvider` or `AddReceiverProvider`.

## Mail Delivery Integration

The real send-and-receive integration test requires these variables on the runner:

- `Mail__ConnectionString`
- `Security__ConnectionString`
- `Security__DecryptionKey`
- `Mail__MicrosoftGraph__TenantId`
- `Mail__MicrosoftGraph__ClientId`
- `Mail__MicrosoftGraph__ClientSecret`
- `Mail__MicrosoftGraph__SendUser`
- `Mail__MicrosoftGraph__ReceiveUser`

The test creates disposable integration databases by appending
`-acceptance-{guid}` to the configured Mail and Security database names.
It sends from `SendUser` to `ReceiveUser`, polls every 10 seconds for up to
120 seconds, and retrieves at most 50 messages.
The Graph application registration must have `Mail.Send` and `Mail.Read` application permissions with admin consent applied.

## Package

The NuGet package produced by this repository is:

- `cCoder.Mail`

## Publishing

GitHub Actions is configured to publish the main package using NuGet trusted publishing.

Before the first publish, configure a trusted publishing policy on nuget.org for:

- Repository owner: `ccoder-co-uk`
- Repository: `cCoder.Mail`
- Workflow file: `publish.yml`

The workflow also expects a `NUGET_USER` repository secret containing the nuget.org profile name used during trusted publishing login.
