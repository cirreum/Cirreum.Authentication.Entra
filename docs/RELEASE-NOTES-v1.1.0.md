# Cirreum.Authentication.Entra 1.1.0 — Entra schemes declare their people

## Why this release exists

The attribute-authority model has providers declare what kind of party they authenticate, so
nothing downstream infers it from whether a token happens to carry a name claim. Entra schemes
validate tokens issued to people; this release says so, per instance, through the registration
funnel.

## What's new

**`SubjectKind.Human`, contributed per instance.** The audience registrar base declares every
Entra instance beside its audience-routing registration — same moment, same anchor, no
per-provider wiring. The instance's optional `ClaimAuthority` block rides the same declaration.

**The Web App cookie scheme is declared.** `AddMicrosoftIdentityWebApp` signs interactive
sessions into the platform-default cookie scheme it registers internally. The cookie is a
continuation — it re-presents the subject the OIDC sign-in established — so the registrar now
declares it `SubjectKind.Unknown`; identical declarations from other instances or providers
dedupe at composition close.

## Compatibility

- **Applications have nothing to change.** Instance configuration and composition are untouched.
- **Registrar hooks changed signature** per the `Cirreum.AuthenticationProvider` 3.0.x contract
  consolidation (`AddAuthenticationForWebApi` / `AddAuthenticationForWebApp` take
  `IAuthenticationBuilder`). Framework-invoked members no application calls directly; shipped as
  a Minor with that scope stated deliberately.
- The declarations are read by higher-layer packages releasing later in the same wave; until
  then they change no behavior.

## See also

- `Cirreum.AuthenticationProvider 3.0.1` — the registration funnel.
- `Cirreum.Kernel 2.1.0` — the `SubjectKind` / `ClaimAuthority` vocabulary.
