# Backlog

Deferred work for **Cirreum.Authentication.Entra**. Items here are tracked but not yet ready
to ship — either because the cost outweighs the benefit in isolation, or
because they're waiting on a forcing function (a related change, a consumer
upgrade, a coordinated multi-repo rollout).

## How this file works

- Each item is a `###` heading so it can be linked to and parsed.
- Each item declares **`SemVer:`** (`Patch` | `Minor` | `Major` | `Unspecified`),
  **`Trigger:`** (the human-readable condition that will make it ready), and
  **`Noted:`** (the date the item was added).
- The Cirreum DevOps release scripts (`PatchRelease`, `MinorRelease`,
  `MajorRelease`) surface items at-or-below the requested bump level so the
  operator can decide whether to fold them in before tagging.
- Items that ship: move from this file to `docs/CHANGELOG.md` under
  `[Unreleased]`. Items that grow into design discussions: promote to an ADR.

## Queued

### Deepen test coverage: registrar wiring

**SemVer:** Patch
**Trigger:** Next substantive change to `EntraAuthenticationRegistrar` (host-shape branching, token
acquisition, or scheme registration).
**Noted:** 2026-07-18 *(shrunk 2026-07-19 — the original item's test project, composition-path tests
for `EnableDownstreamApi` / `EntraDownstreamRegistration`, and settings-binding coverage shipped;
`EnableDownstreamApi` re-registration pinned as last-wins.)*

The remaining untested surface is `EntraAuthenticationRegistrar` itself: per-instance scheme
registration, the Web API vs Web App host-shape branch, and the downstream-API enablement path
(`EnableTokenAcquisitionToCallDownstreamApi` + `InvokeOnce` on the converged builder). Needs a
harness that exercises Microsoft.Identity.Web registration without a live IdP — verify against the
service collection (registered schemes/options), not network behavior.
