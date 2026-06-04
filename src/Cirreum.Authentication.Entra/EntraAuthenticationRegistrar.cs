namespace Cirreum.Authentication;

using Cirreum.AuthenticationProvider;
using Cirreum.Authentication.Configuration;

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
		authBuilder.AddMicrosoftIdentityWebApi(
					instanceSection,
					jwtBearerScheme: providerSettings.Scheme);
	}

	/// <inheritdoc/>
	public override void AddAuthenticationForWebApp(IConfigurationSection instanceSection,
		EntraAuthenticationInstanceSettings providerSettings,
		AuthenticationBuilder authBuilder) {
		authBuilder.AddMicrosoftIdentityWebApp(
					instanceSection,
					openIdConnectScheme: providerSettings.Scheme);
	}

}