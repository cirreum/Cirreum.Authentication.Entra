namespace Cirreum.Authentication.Configuration;

using Cirreum.AuthenticationProvider.Configuration;

public class EntraAuthenticationInstanceSettings
	: AudienceAuthenticationProviderInstanceSettings {

	/// <summary>
	/// Gets or sets the Azure AD tenant ID.
	/// </summary>
	public string TenantId { get; set; } = "";

	/// <summary>
	/// Gets or sets the Azure AD client (application) ID.
	/// </summary>
	public string ClientId { get; set; } = "";

	/// <summary>
	/// Gets or sets the Azure AD instance URL.
	/// Defaults to the public Azure cloud.
	/// </summary>
	public string Instance { get; set; } = "https://login.microsoftonline.com/";

	/// <summary>
	/// Scopes requested up-front during interactive sign-in (Web App host) so tokens for downstream APIs
	/// are available without re-prompting. Ignored for a Web API host, where on-behalf-of acquisition
	/// obtains downstream tokens on demand. Consumed only when the app registers a downstream-API callback
	/// via <c>EnableDownstreamApi(...)</c>.
	/// </summary>
	public string[] InitialScopes { get; set; } = [];

}