# cCoder.Mail

`cCoder.Mail` contains the Mail domain for the cCoder platform. It provides mail-server configuration, queued email, sent email, event handling, and the background sender loop used by cCoder applications.

## Functionality

- Mail server management: configure application-owned SMTP settings, including host, port, SSL, sender, and credentials.
- Queued email management: create and inspect pending outbound emails.
- Sent email management: inspect emails that have been successfully dispatched.
- Sender hosted service: checks the queue every minute and attempts SMTP delivery for pending messages.
- App lifecycle event handling: listens for app add, update, and delete events so mail-owned app data stays aligned.
- Manual test UI: `/tools/index.html` provides a lightweight CRUD surface for mail servers, with queued and sent mail managed inside the selected server context.
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

## Build

```powershell
dotnet build src/cCoder.Mail.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Mail.sln -v minimal --no-build
```

The solution test run includes unit tests and both app acceptance suites. Acceptance tests actively call the hosted HTTP surfaces, including health endpoints and the manual tools shell.

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
