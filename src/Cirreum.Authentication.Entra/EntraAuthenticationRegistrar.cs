namespace Cirreum.Authentication;

using Cirreum.AuthenticationProvider;
using Cirreum.Authentication.Configuration;
using Cirreum.Security;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;

/// <summary>
/// Registrar for Entra (Azure AD) authorization provider instances.
/// </summary>
public sealed class EntraAuthenticationRegistrar
	: AudienceAuthenticationProviderRegistrar<
		EntraAuthenticationSettings,
		EntraAuthenticationInstanceSettings> {

	public override string ProviderName => "Entra";

	/// <inheritdoc/>
	public override SubjectKind SubjectKind => SubjectKind.Human;

	/// <inheritdoc/>
	public override void ValidateSettings(EntraAuthenticationInstanceSettings settings) {

		if (string.IsNullOrWhiteSpace(settings.TenantId)) {
			throw new InvalidOperationException(
				$"Entra provider instance '{settings.Scheme}' requires a TenantId.");
		}

		if (string.IsNullOrWhiteSpace(settings.ClientId)) {
			throw new InvalidOperationException(
				$"Entra provider instance '{settings.Scheme}' requires a ClientId.");
		}

	}

	/// <inheritdoc/>
	public override void AddAuthenticationForWebApi(IConfigurationSection instanceSection,
		EntraAuthenticationInstanceSettings providerSettings,
		AuthenticationBuilder authBuilder) {

		var identityApi = authBuilder.AddMicrosoftIdentityWebApi(
			instanceSection,
			jwtBearerScheme: providerSettings.Scheme);

		// If the app registered a downstream-API callback (auth.EnableDownstreamApi(...)), enable
		// on-behalf-of token acquisition for THIS scheme, then run the app's app-wide extras once.
		var downstream = EntraDownstreamRegistration.GetOrAdd(authBuilder.Services);
		if (downstream.Callback is not null) {
			downstream.InvokeOnce(identityApi.EnableTokenAcquisitionToCallDownstreamApi());
		}
	}

	/// <inheritdoc/>
	public override void AddAuthenticationForWebApp(IConfigurationSection instanceSection,
		EntraAuthenticationInstanceSettings providerSettings,
		AuthenticationBuilder authBuilder) {

		var identityApp = authBuilder.AddMicrosoftIdentityWebApp(
			instanceSection,
			openIdConnectScheme: providerSettings.Scheme);

		// Web App host: token acquisition pre-requests the instance's InitialScopes at interactive sign-in.
		var downstream = EntraDownstreamRegistration.GetOrAdd(authBuilder.Services);
		if (downstream.Callback is not null) {
			downstream.InvokeOnce(identityApp.EnableTokenAcquisitionToCallDownstreamApi(providerSettings.InitialScopes));
		}
	}

}