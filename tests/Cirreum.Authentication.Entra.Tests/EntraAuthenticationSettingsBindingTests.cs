namespace Cirreum.Authentication.Entra.Tests;

using Cirreum.Authentication.Configuration;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Proofs that the Entra provider settings bind from configuration the way the registrar consumes
/// them — instances keyed by name, with the Entra-specific knobs (tenant/client/instance/scopes)
/// and their defaults surviving when absent.
/// </summary>
public sealed class EntraAuthenticationSettingsBindingTests {

	private static EntraAuthenticationSettings Bind(Dictionary<string, string?> values) =>
		new ConfigurationBuilder().AddInMemoryCollection(values).Build().Get<EntraAuthenticationSettings>()!;

	[Fact]
	public void Instance_settings_bind_from_the_instances_section() {
		var settings = Bind(new Dictionary<string, string?> {
			["Instances:Default:TenantId"] = "11111111-2222-3333-4444-555555555555",
			["Instances:Default:ClientId"] = "66666666-7777-8888-9999-000000000000",
			["Instances:Default:InitialScopes:0"] = "api://downstream/.default",
			["Instances:Default:InitialScopes:1"] = "User.Read",
		});

		var instance = settings.Instances.Should().ContainKey("Default").WhoseValue;
		instance.TenantId.Should().Be("11111111-2222-3333-4444-555555555555");
		instance.ClientId.Should().Be("66666666-7777-8888-9999-000000000000");
		instance.InitialScopes.Should().Equal("api://downstream/.default", "User.Read");
	}

	[Fact]
	public void Instance_url_defaults_to_the_public_cloud_when_absent() {
		var settings = Bind(new Dictionary<string, string?> {
			["Instances:Default:TenantId"] = "tid",
		});

		settings.Instances["Default"].Instance.Should().Be("https://login.microsoftonline.com/");
		settings.Instances["Default"].InitialScopes.Should().BeEmpty();
	}
}
