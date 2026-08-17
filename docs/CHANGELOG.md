# Cirreum.Authentication.Entra Changelog

All notable changes to **Cirreum.Authentication.Entra** are documented in this file.

Format: [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) — [SemVer](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Added

- **Declares `SubjectKind.Human`.** Entra schemes validate tokens issued to people, so nothing
  downstream has to infer it from whether a token happens to carry a name claim. The declaration
  is contributed per instance by the audience registrar base (the registration funnel,
  `Cirreum.AuthenticationProvider` 3.0.1).
- **The Web App cookie scheme is declared.** `AddMicrosoftIdentityWebApp` signs interactive
  sessions into the platform-default cookie scheme; the registrar now declares it
  `SubjectKind.Unknown` — a continuation re-presenting the subject the OIDC sign-in established.

### Changed

- Registrar hooks take `IAuthenticationBuilder` per the `Cirreum.AuthenticationProvider` 3.0.1
  contract consolidation. Registrar plumbing only; not app-facing surface.

### Updated

- Updated NuGet packages.

## [1.0.12] - 2026-08-04

### Updated

- Updated NuGet packages (Cirreum spine 4.2.0 wave: `Cirreum.Contracts` 4.2.0 / `Cirreum.Domain` 4.2.0 and current patch releases).

## [1.0.11] - 2026-07-31

### Updated

- Updated NuGet packages (Cirreum spine 4.0.1 wave: `Cirreum.Contracts` 4.0.1 / `Cirreum.Domain` 4.0.1 / `Cirreum.Kernel` 2.0.1 / `Cirreum.AuthenticationProvider` 2.0.3).

## [1.0.10] - 2026-07-30

### Updated

- Updated `Microsoft.Identity.Web` `4.14.1` → `4.14.2`.

## [1.0.9] - 2026-07-29

### Updated

- Updated NuGet packages.

## [1.0.8] - 2026-07-27

### Updated

- Updated NuGet packages.

## [1.0.7] - 2026-07-24

### Updated

- Updated NuGet packages.

## [1.0.6] - 2026-07-20

### Updated

- Updated NuGet packages.

## [1.0.5] - 2026-07-19

### Updated

- Updated NuGet packages.

## [1.0.1] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.0] - 2026-07-03

### Added

- Initial release. Azure Entra ID authentication scheme of the Cirreum framework, established as part of the **Cirreum 1.0 Foundation Reset** wave.
- **Renamed and re-homed from the deprecated `Cirreum.Authorization.Entra`** per the Three Security Pillars separation. Microsoft.Identity.Web wraps both `AddMicrosoftIdentityWebApi` (Web API JWT) and `AddMicrosoftIdentityWebApp` (Web App OIDC); both are authentication concerns, not authorization.
- Surface preserved from 1.0.x of the predecessor package:
  - `EntraAuthenticationRegistrar` extends `AudienceAuthenticationProviderRegistrar`
  - `EntraAuthenticationInstanceSettings` with `TenantId` + `ClientId` (the audience)
  - `EntraAuthenticationSettings` collection
  - Web API (JWT bearer) and Web App (OIDC) wiring via Microsoft.Identity.Web
- Audience-claim dispatch via the dynamic forward resolver.

### Changed

- Dropped redundant explicit `Microsoft.AspNetCore.DataProtection` package reference (transitively present via Microsoft.Identity.Web; NU1510 cleanup).

### Migration

Apps consuming `Cirreum.Authorization.Entra` migrate by installing `Cirreum.Authentication.Entra` and switching their composition root from `AddAuthorization(...)` to `AddAuthentication(...)`. See [`docs/MIGRATION-v1.md`](MIGRATION-v1.md).
