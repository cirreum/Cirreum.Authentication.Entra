# Cirreum.Authentication.Entra 1.0.0 — Renamed home for Entra ID

## Why this release exists

Microsoft Entra ID integration via Microsoft.Identity.Web validates JWTs and runs the OpenID Connect flow — both are authentication. The **Cirreum 1.0 Foundation Reset** moves this package from the deprecated `Cirreum.Authorization.Entra` to its correct home under the Authentication pillar.

## What's new

This is a rename release. The Entra integration is unchanged.

## What's preserved

- `AddMicrosoftIdentityWebApi(...)` for Web API JWT validation
- `AddMicrosoftIdentityWebApp(...)` for Web App OpenID Connect
- `TenantId` + `ClientId` strongly-typed required-field validation
- Configuration binding to Microsoft.Identity.Web's options shape

## Compatibility

- **.NET 10.0** target.
- **Microsoft.Identity.Web 4.9.0+**.
- **Cirreum.AuthenticationProvider 1.0.0+**.
- Apps migrating from `Cirreum.Authorization.Entra` follow [`MIGRATION-v1.md`](MIGRATION-v1.md).

## See also

- [`MIGRATION-v1.md`](MIGRATION-v1.md), [`CHANGELOG.md`](CHANGELOG.md)
