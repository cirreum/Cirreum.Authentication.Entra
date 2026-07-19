namespace Cirreum.Authentication.Entra.Tests;

using Cirreum.AuthenticationProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

/// <summary>
/// Composition-path proofs for <c>EnableDownstreamApi(...)</c> and its backing
/// <see cref="EntraDownstreamRegistration"/> composition state: the callback must land on the
/// found-or-create holder (last-wins on re-registration) and must be invoked exactly once by
/// <c>InvokeOnce</c>. Guards the untested-composition-verb escape vector behind
/// Cirreum.Authentication.ApiKey issue #1, where the provider's composition verb threw unconditionally
/// through five published versions because no test ever invoked it.
/// </summary>
public sealed class EntraDownstreamApiCompositionTests {

	private static IAuthenticationBuilder CreateBuilder(IServiceCollection services) {
		var builder = Substitute.For<IAuthenticationBuilder>();
		builder.Services.Returns(services);
		builder.AuthBuilder.Returns(new AuthenticationBuilder(services));
		builder.Configuration.Returns(new ConfigurationBuilder().Build());
		return builder;
	}

	[Fact]
	public void EnableDownstreamApi_stores_the_callback_on_the_registration() {
		var services = new ServiceCollection();
		var builder = CreateBuilder(services);
		Action<MicrosoftIdentityAppCallsWebApiAuthenticationBuilder> callback = _ => { };

		builder.EnableDownstreamApi(callback);

		EntraDownstreamRegistration.GetOrAdd(services).Callback.Should().BeSameAs(callback);
	}

	[Fact]
	public void EnableDownstreamApi_called_twice_is_last_wins() {
		var services = new ServiceCollection();
		var builder = CreateBuilder(services);
		Action<MicrosoftIdentityAppCallsWebApiAuthenticationBuilder> first = _ => { };
		Action<MicrosoftIdentityAppCallsWebApiAuthenticationBuilder> second = _ => { };

		builder.EnableDownstreamApi(first);
		builder.EnableDownstreamApi(second);

		EntraDownstreamRegistration.GetOrAdd(services).Callback.Should().BeSameAs(second);
	}

	[Fact]
	public void EnableDownstreamApi_throws_on_null_arguments() {
		var services = new ServiceCollection();
		var builder = CreateBuilder(services);
		IAuthenticationBuilder nullBuilder = null!;

		((Action)(() => nullBuilder.EnableDownstreamApi(_ => { }))).Should().Throw<ArgumentNullException>();
		((Action)(() => builder.EnableDownstreamApi(null!))).Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void GetOrAdd_finds_or_creates_a_single_holder() {
		var services = new ServiceCollection();

		var first = EntraDownstreamRegistration.GetOrAdd(services);
		var second = EntraDownstreamRegistration.GetOrAdd(services);

		second.Should().BeSameAs(first);
		services.Count(d => d.ServiceType == typeof(EntraDownstreamRegistration)).Should().Be(1);
	}

	[Fact]
	public void InvokeOnce_invokes_the_callback_exactly_once() {
		var registration = new EntraDownstreamRegistration();
		var invocations = 0;
		registration.Callback = _ => invocations++;

		registration.InvokeOnce(null!);
		registration.InvokeOnce(null!);

		invocations.Should().Be(1);
	}

	[Fact]
	public void InvokeOnce_is_a_no_op_without_a_callback() {
		var registration = new EntraDownstreamRegistration();

		var act = () => registration.InvokeOnce(null!);

		act.Should().NotThrow();
	}
}
