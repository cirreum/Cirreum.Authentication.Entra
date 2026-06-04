# Migration to Cirreum.Authentication.Entra v1.0

**From:** `Cirreum.Authorization.Entra 1.0.x` (now deprecated)
**To:** `Cirreum.Authentication.Entra 1.0.0`

## Why v1

Microsoft.Identity.Web's `AddMicrosoftIdentityWebApi` and `AddMicrosoftIdentityWebApp` validate JWTs and run the OIDC flow — both are authentication, not authorization. The **Cirreum 1.0 Foundation Reset** moves this package to its correct pillar.

## Breaking Changes — Find/Replace Table

| Before | After |
|---|---|
| `<PackageReference Include="Cirreum.Authorization.Entra" ... />` | `<PackageReference Include="Cirreum.Authentication.Entra" ... />` |
| `EntraAuthorizationRegistrar` | `EntraAuthenticationRegistrar` |
| `EntraAuthorizationInstanceSettings` | `EntraAuthenticationInstanceSettings` |
| `EntraAuthorizationSettings` | `EntraAuthenticationSettings` |
| `AddAuthorization(authz => authz.AddEntra(...))` | `AddAuthentication(auth => auth.AddEntra(...))` |
| `Cirreum:Authorization:Providers:Entra:Instances:{name}` | `Cirreum:Authentication:Providers:Entra:Instances:{name}` |

## What Didn't Change

- Microsoft.Identity.Web integration behavior (Web API + Web App branches)
- `TenantId` / `ClientId` validation
- Configuration binding to Microsoft.Identity.Web's settings shape

## Migration Walkthrough

1. **Update `<PackageReference>`** in your csproj.
2. Apply the find/replace table above.
3. **Update `appsettings.json`** configuration root.
4. **Move the `AddEntra` call** from `AddAuthorization(...)` to `AddAuthentication(...)`.
5. Rebuild and verify.
