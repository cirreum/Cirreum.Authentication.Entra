namespace Cirreum.Authentication.Entra.Tests;

using Cirreum.Authentication;
using Cirreum.Authentication.Configuration;
using Cirreum.AuthenticationProvider;
using Cirreum.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

/// <summary>
/// Registration-path proofs for <see cref="EntraAuthenticationRegistrar"/>.
/// </summary>
/// <remarks>
/// <para>
/// The two host-shape methods are exercised directly rather than through the base registrar's
/// <c>RegisterScheme</c>. That branch reads <c>ProviderContext.GetRuntimeType()</c>, a write-once
/// static with no reset — one test could set it, and every other test in the assembly would then be
/// stuck with that host shape. It also belongs to <c>AudienceAuthenticationProviderRegistrar</c> in
/// Cirreum.AuthenticationProvider, not to this package. Calling the public methods keeps each test
/// independent and scopes them to what Entra actually contributes.
/// </para>
/// <para>
/// Assertions read the service collection, never network behaviour: what matters here is that a
/// scheme is registered under the configured instance key, and that the downstream-API callback is
/// invoked exactly when one was supplied.
/// </para>
/// </remarks>
public sealed class EntraAuthenticationRegistrarTests {

	private const string Scheme = "corporate";

	private static EntraAuthenticationInstanceSettings Settings(
		string tenantId = "11111111-1111-1111-1111-111111111111",
		string clientId = "22222222-2222-2222-2222-222222222222",
		string[]? initialScopes = null) =>
		new() {
			Scheme = Scheme,
			Audience = "api://corporate",
			TenantId = tenantId,
			ClientId = clientId,
			InitialScopes = initialScopes ?? []
		};

	// AddAuthentication() rather than new AuthenticationBuilder(services): Microsoft.Identity.Web's
	// registration depends on what it brings along (TimeProvider, data protection, encoders).
	private static (IServiceCollection Services, IAuthenticationBuilder AuthBuilder) NewComposition() {
		var services = new ServiceCollection();
		services.AddLogging();
		return (services, new TestAuthenticationBuilder(
			services,
			services.AddAuthentication(),
			new ConfigurationBuilder().Build()));
	}

	private sealed class TestAuthenticationBuilder(
		IServiceCollection services,
		AuthenticationBuilder authBuilder,
		IConfiguration configuration) : IAuthenticationBuilder {

		public IServiceCollection Services { get; } = services;
		public AuthenticationBuilder AuthBuilder { get; } = authBuilder;
		public IConfiguration Configuration { get; } = configuration;

		public IAuthenticationBuilder DeclareScheme(string scheme, SubjectKind subjectKind,
			ClaimAuthority profile = ClaimAuthority.Unspecified,
			ClaimAuthority roles = ClaimAuthority.Unspecified) {
			this.Services.AddSingleton(new SchemeClaimAuthorityRegistration(scheme, subjectKind, profile, roles));
			return this;
		}

		public IAuthenticationBuilder AddScheme<TOptions, THandler>(string scheme, SubjectKind subjectKind,
			ClaimAuthority profile = ClaimAuthority.Unspecified,
			ClaimAuthority roles = ClaimAuthority.Unspecified,
			Action<TOptions>? configureOptions = null)
			where TOptions : AuthenticationSchemeOptions, new()
			where THandler : AuthenticationHandler<TOptions> {
			this.DeclareScheme(scheme, subjectKind, profile, roles);
			this.AuthBuilder.AddScheme<TOptions, THandler>(scheme, configureOptions);
			return this;
		}
	}

	private static IConfigurationSection EmptySection() =>
		new ConfigurationBuilder().Build().GetSection("Cirreum:Authentication:Providers:Entra:Instances:corporate");

	private static IReadOnlyList<string> RegisteredSchemes(IServiceCollection services) {
		using var provider = services.BuildServiceProvider();
		return [.. provider.GetRequiredService<IOptions<AuthenticationOptions>>().Value.SchemeMap.Keys];
	}

	// -------------------------------------------------------------------------
	// Settings validation
	// -------------------------------------------------------------------------

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void A_missing_tenant_id_is_refused(string tenantId) {
		var act = () => new EntraAuthenticationRegistrar().ValidateSettings(Settings(tenantId: tenantId));

		act.Should().Throw<InvalidOperationException>().WithMessage($"*{Scheme}*TenantId*");
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void A_missing_client_id_is_refused(string clientId) {
		var act = () => new EntraAuthenticationRegistrar().ValidateSettings(Settings(clientId: clientId));

		act.Should().Throw<InvalidOperationException>().WithMessage($"*{Scheme}*ClientId*");
	}

	[Fact]
	public void A_complete_instance_validates() {
		var act = () => new EntraAuthenticationRegistrar().ValidateSettings(Settings());

		act.Should().NotThrow();
	}

	// -------------------------------------------------------------------------
	// Scheme name == instance key
	// -------------------------------------------------------------------------

	[Fact]
	public void The_web_api_host_registers_the_scheme_under_the_instance_key() {
		// The framework contract is that the instance key IS the scheme name: the AuthenticatedScheme
		// stamp, IApplicationUserResolver dispatch and boundary resolution all key off it, so a
		// registrar that registers under anything else breaks that dispatch with no error raised.
		var (services, authBuilder) = NewComposition();

		new EntraAuthenticationRegistrar()
			.AddAuthenticationForWebApi(EmptySection(), Settings(), authBuilder);

		RegisteredSchemes(services).Should().Contain(Scheme);
	}

	[Fact]
	public void The_web_app_host_registers_the_scheme_under_the_instance_key() {
		var (services, authBuilder) = NewComposition();

		new EntraAuthenticationRegistrar()
			.AddAuthenticationForWebApp(EmptySection(), Settings(), authBuilder);

		RegisteredSchemes(services).Should().Contain(Scheme);
	}

	[Fact]
	public void The_web_app_host_declares_the_cookie_scheme_as_a_continuation() {
		// AddMicrosoftIdentityWebApp signs interactive sessions into the platform-default cookie
		// scheme. The cookie re-presents the subject the OIDC sign-in established, so it declares
		// Unknown — the origin's declaration governs, not the transport's.
		var (services, authBuilder) = NewComposition();

		new EntraAuthenticationRegistrar()
			.AddAuthenticationForWebApp(EmptySection(), Settings(), authBuilder);

		services
			.Select(d => d.ImplementationInstance)
			.OfType<SchemeClaimAuthorityRegistration>()
			.Where(r => r.Scheme == CookieAuthenticationDefaults.AuthenticationScheme)
			.Should().ContainSingle()
			.Which.SubjectKind.Should().Be(SubjectKind.Unknown);
	}

	// -------------------------------------------------------------------------
	// Downstream-API enablement
	// -------------------------------------------------------------------------

	[Fact]
	public void The_web_api_host_enables_downstream_acquisition_when_a_callback_is_registered() {
		var (services, authBuilder) = NewComposition();
		var invocations = 0;
		EntraDownstreamRegistration.GetOrAdd(services).Callback = _ => invocations++;

		new EntraAuthenticationRegistrar()
			.AddAuthenticationForWebApi(EmptySection(), Settings(), authBuilder);

		invocations.Should().Be(1);
	}

	[Fact]
	public void The_web_app_host_enables_downstream_acquisition_when_a_callback_is_registered() {
		var (services, authBuilder) = NewComposition();
		var invocations = 0;
		EntraDownstreamRegistration.GetOrAdd(services).Callback = _ => invocations++;

		new EntraAuthenticationRegistrar()
			.AddAuthenticationForWebApp(EmptySection(), Settings(initialScopes: ["api://downstream/.default"]), authBuilder);

		invocations.Should().Be(1);
	}

	[Fact]
	public void No_downstream_callback_means_acquisition_is_never_enabled() {
		// The holder exists only once EnableDownstreamApi(...) has been called. Enabling token
		// acquisition regardless would pull the confidential-client machinery into an app that never
		// asked to call a downstream API.
		var (services, authBuilder) = NewComposition();

		var act = () => new EntraAuthenticationRegistrar()
			.AddAuthenticationForWebApi(EmptySection(), Settings(), authBuilder);

		act.Should().NotThrow();
		EntraDownstreamRegistration.GetOrAdd(services).Callback.Should().BeNull();
	}

	[Fact]
	public void A_second_instance_registers_its_own_scheme_but_runs_the_callback_once() {
		// The app-wide downstream extras are the app's, not the instance's — two configured Entra
		// instances must not run them twice.
		var (services, authBuilder) = NewComposition();
		var invocations = 0;
		EntraDownstreamRegistration.GetOrAdd(services).Callback = _ => invocations++;

		var registrar = new EntraAuthenticationRegistrar();
		registrar.AddAuthenticationForWebApi(EmptySection(), Settings(), authBuilder);

		var second = Settings();
		second.Scheme = "partner";
		second.Audience = "api://partner";
		registrar.AddAuthenticationForWebApi(EmptySection(), second, authBuilder);

		invocations.Should().Be(1);
		RegisteredSchemes(services).Should().Contain([Scheme, "partner"]);
	}
}
