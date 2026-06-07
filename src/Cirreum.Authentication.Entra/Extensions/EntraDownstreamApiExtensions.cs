namespace Cirreum.Authentication;

using Cirreum.AuthenticationProvider;
using Microsoft.Identity.Web;

/// <summary>
/// Contributes the <c>EnableDownstreamApi(...)</c> composition seam for the Entra provider, available
/// inside the <c>configure</c> callback of the umbrella <c>AddAuthentication(...)</c> (same shape as the
/// scheme verbs <c>AddApiKey</c>/<c>AddSignedRequest</c>: an extension on <see cref="IAuthenticationBuilder"/>).
/// </summary>
public static class EntraDownstreamApiExtensions {

	/// <summary>
	/// Registers an Entra downstream-API configuration callback. When present, every configured Entra
	/// instance enables Microsoft.Identity.Web token acquisition — on-behalf-of for a Web API host, or
	/// auth-code with the instance's <c>InitialScopes</c> for a Web App host — and the callback runs once
	/// on the converged <see cref="MicrosoftIdentityAppCallsWebApiAuthenticationBuilder"/> so the app can
	/// add token caches, downstream API clients, or Microsoft Graph identically for either host shape.
	/// Omit it entirely when no scheme calls a downstream API (the default bare registration is unchanged).
	/// </summary>
	/// <param name="builder">The Cirreum authentication builder.</param>
	/// <param name="configure">Configures the converged builder, e.g.
	/// <c>c =&gt; c.AddDistributedTokenCaches().AddMicrosoftGraph(section)</c>.</param>
	/// <returns>The builder for chaining.</returns>
	public static IAuthenticationBuilder EnableDownstreamApi(
		this IAuthenticationBuilder builder,
		Action<MicrosoftIdentityAppCallsWebApiAuthenticationBuilder> configure) {

		ArgumentNullException.ThrowIfNull(builder);
		ArgumentNullException.ThrowIfNull(configure);

		EntraDownstreamRegistration.GetOrAdd(builder.Services).Callback = configure;
		return builder;
	}

}
