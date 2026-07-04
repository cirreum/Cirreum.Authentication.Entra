# Cirreum.Authentication.Entra Changelog

All notable changes to **Cirreum.Authentication.Entra** are documented in this file.

Format: [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) — [SemVer](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

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
