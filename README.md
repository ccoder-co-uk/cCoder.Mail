# cCoder.Mail

`cCoder.Mail` contains the Mail domain for the cCoder platform. It provides mail-server configuration, queued email, sent email, mailbox receive support, event handling, and the background sender loop used by cCoder applications.

## Functionality

- Mail server management: configure application-owned SMTP settings, including host, port, SSL, sender, and credentials.
- Queued email management: create and inspect pending outbound emails.
- Sent email management: inspect emails that have been successfully dispatched.
- Mail client abstraction: `IMailClient` sends queued mail and can receive mailbox messages for a requested time period.
- Received email inspection: `ReceivedEmailController` can fetch mailbox messages without persisting them.
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
  Optional end-to-end tests that send queued mail through SMTP and receive it back from a POP3 mailbox.

## Build

```powershell
dotnet build src/cCoder.Mail.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Mail.sln -v minimal --no-build
```

The solution test run includes unit tests, app acceptance suites, and the optional mail delivery integration suite. Acceptance tests actively call the hosted HTTP surfaces, including health endpoints and the manual tools shell.

The end-to-end mail delivery test is disabled unless `CCODER_MAIL_INTEGRATION_ENABLED=true`. When enabled, it queues an email through `Mail.Web`, runs the sender orchestration, then calls the received-mail API until the same message is visible in the mailbox.

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
- `/Api/Core/ReceivedEmail/Receive` fetches mailbox messages using the supplied host, port, SSL, credentials, and date range.
- `/Api/Mail/ReceivedEmail/Receive` exposes the same receive endpoint on the Mail route.

Useful `Mail.HostedServices` endpoints:

- `/` returns a plain-text hosted-services report.
- `/Health` returns `Healthy`.

## Local Configuration

The runnable apps read local secrets from environment variables rather than committed config.

Before running `src/Mail.Web` or `src/Mail.HostedServices`, set:

- `ConnectionStrings__Core`

Before running `src/Mail.Web`, also set:

- `ConnectionStrings__SSO`
- `Settings__DecryptionKey`

The committed `appsettings.json` keeps these values blank so user or machine environment variables can supply them during local development.

## Mail Delivery Integration

To enable the real send-and-receive integration test, set these variables on the runner:

- `CCODER_MAIL_INTEGRATION_ENABLED=true`
- `CCODER_ACCEPTANCE_CORE_CONNECTION_STRING`
- `CCODER_ACCEPTANCE_SSO_CONNECTION_STRING`
- `CCODER_MAIL_INTEGRATION_SMTP_HOST`
- `CCODER_MAIL_INTEGRATION_SMTP_PORT` (defaults to `587`)
- `CCODER_MAIL_INTEGRATION_SMTP_SSL` (defaults to `true`)
- `CCODER_MAIL_INTEGRATION_SMTP_USER`
- `CCODER_MAIL_INTEGRATION_SMTP_PASSWORD`
- `CCODER_MAIL_INTEGRATION_SMTP_FROM` (defaults to `CCODER_MAIL_INTEGRATION_SMTP_USER`)
- `CCODER_MAIL_INTEGRATION_POP_HOST`
- `CCODER_MAIL_INTEGRATION_POP_PORT` (defaults to `995`)
- `CCODER_MAIL_INTEGRATION_POP_SSL` (defaults to `true`)
- `CCODER_MAIL_INTEGRATION_POP_USER` (defaults to `CCODER_MAIL_INTEGRATION_SMTP_USER`)
- `CCODER_MAIL_INTEGRATION_POP_PASSWORD` (defaults to `CCODER_MAIL_INTEGRATION_SMTP_PASSWORD`)
- `CCODER_MAIL_INTEGRATION_TO` (defaults to `CCODER_MAIL_INTEGRATION_POP_USER`)
- `CCODER_MAIL_INTEGRATION_MAX_MESSAGES` (defaults to `50`)
- `CCODER_MAIL_INTEGRATION_RECEIVE_TIMEOUT_SECONDS` (defaults to `120`)
- `CCODER_MAIL_INTEGRATION_RECEIVE_POLL_SECONDS` (defaults to `10`)

The test creates disposable integration databases by appending `-mail-integration` to the acceptance Core and SSO database names.

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
