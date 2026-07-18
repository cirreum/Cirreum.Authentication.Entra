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

### Test project (none exists) + composition-path coverage for `EnableDownstreamApi`

**SemVer:** Patch
**Trigger:** Next substantive change to this package (do not ship another feature on an untested package).
**Noted:** 2026-07-18

This repo has **no test project at all**. A 2026-07-18 sweep (prompted by
Cirreum.Authentication.ApiKey issue #1, where the untested `AddApiKey()` composition verb threw
unconditionally through five published versions) statically audited `EnableDownstreamApi` and
found **no defect** — the verb only stores the configure callback on the
`EntraDownstreamRegistration` composition-state object, with no registration shape that can throw
at composition time. This item is the coverage debt: scaffold the test project from
`C:\Cirreum\DevOps\templates` (xUnit + FluentAssertions + NSubstitute per house convention) and
include a composition-path test for the verb (callback lands on the registration object; calling
it twice — last-wins or guard, whichever the design intends — is pinned by a test) alongside
first component coverage for the Entra registrar/instance-settings paths that don't require a
live IdP.
