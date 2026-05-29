# cCoder.Mail

`cCoder.Mail` contains the Mail domain for the cCoder platform.

## Contents

- `src/cCoder.Mail`
  The main library package published to NuGet.
- `src/Mail.Web`
  The standalone web host for the Mail domain.
- `src/cCoder.Mail.Tests`
  Unit tests for the domain.
- `src/Mail.AcceptanceTests`
  Acceptance tests for the standalone host.

## Build

```powershell
dotnet build src/cCoder.Mail.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Mail.sln -v minimal --no-build
```

## Local Configuration

The standalone web host reads local secrets from environment variables rather than committed config.

Before running `src/Mail.Web`, set:

- `ConnectionStrings__Core`
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
