# Cirreum Authentication - Entra ID

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Authentication.Entra.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Authentication.Entra/)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Authentication.Entra?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Authentication.Entra/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Azure Entra ID authentication scheme for the Cirreum framework**

> **Migrating from `Cirreum.Authorization.Entra`?** Renamed successor. See [`docs/MIGRATION-v1.md`](docs/MIGRATION-v1.md).

## Overview

**Cirreum.Authentication.Entra** integrates Azure Entra ID (formerly Azure AD) authentication via Microsoft.Identity.Web. Validates JWTs from Entra tenants for Web API endpoints; runs the OpenID Connect flow for Web App scenarios.

For Entra External ID (B2C-style, customer-facing IdP), use [`Cirreum.Authentication.External`](https://github.com/cirreum/Cirreum.Authentication.External) (which supports BYOID / multi-tenant routing) or generic [`Cirreum.Authentication.Oidc`](https://github.com/cirreum/Cirreum.Authentication.Oidc) depending on the topology.

## Installation

```bash
dotnet add package Cirreum.Authentication.Entra
```

## Configuration

```json
{
  "Cirreum": {
    "Authentication": {
      "Providers": {
        "Entra": {
          "Instances": {
            "operator-tenant": {
              "Enabled": true,
              "TenantId": "00000000-0000-0000-0000-000000000000",
              "ClientId": "11111111-1111-1111-1111-111111111111",
              "Audience": "api://my-api-app",
              "Instance": "https://login.microsoftonline.com/"
            }
          }
        }
      }
    }
  }
}
```

The instance section binds directly to Microsoft.Identity.Web's settings shape — any property recognized by `AddMicrosoftIdentityWebApi` / `AddMicrosoftIdentityWebApp` flows through.

## See also

- [Microsoft.Identity.Web documentation](https://learn.microsoft.com/azure/active-directory/develop/microsoft-identity-web)

## License

MIT — see [LICENSE](LICENSE).

---

**Cirreum Foundation Framework**
*Layered simplicity for modern .NET*
