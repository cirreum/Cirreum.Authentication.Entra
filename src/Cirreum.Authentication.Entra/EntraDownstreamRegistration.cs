namespace Cirreum.Authentication;

using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

/// <summary>
/// App-wide holder for the optional Entra downstream-API configuration callback supplied via
/// <c>EnableDownstreamApi(...)</c>. It is stashed on the service collection — the same found-or-create
/// pattern the ApiKey provider uses for its composition state — so the auto-registered
/// <see cref="EntraAuthenticationRegistrar"/> can read it at registration time without coupling the
/// generic, provider-agnostic <c>RegisterAuthenticationProvider</c> helper to Entra.
/// </summary>
internal sealed class EntraDownstreamRegistration {

	private bool _invoked;

	/// <summary>
	/// The app-supplied downstream-API configuration, or <see langword="null"/> when none was registered.
	/// </summary>
	public Action<MicrosoftIdentityAppCallsWebApiAuthenticationBuilder>? Callback { get; set; }

	/// <summary>
	/// Invokes the app callback exactly once. Token acquisition is enabled per scheme (per Entra instance),
	/// but the app's extras (token caches, downstream API clients, Microsoft Graph) are app-wide and must be
	/// added a single time — the converged builder of the first instance serves every scheme.
	/// </summary>
	/// <param name="builder">The converged builder produced by <c>EnableTokenAcquisitionToCallDownstreamApi</c>.</param>
	public void InvokeOnce(MicrosoftIdentityAppCallsWebApiAuthenticationBuilder builder) {
		if (this._invoked || this.Callback is null) {
			return;
		}

		this._invoked = true;
		this.Callback(builder);
	}

	/// <summary>Finds or creates the singleton holder on the service collection.</summary>
	public static EntraDownstreamRegistration GetOrAdd(IServiceCollection services) {
		var existing = services.FirstOrDefault(d => d.ServiceType == typeof(EntraDownstreamRegistration));
		if (existing?.ImplementationInstance is EntraDownstreamRegistration registration) {
			return registration;
		}

		registration = new EntraDownstreamRegistration();
		services.AddSingleton(registration);
		return registration;
	}

}
