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

}