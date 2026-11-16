using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

/// <summary>
/// Describes how the service containers of an integration test assembly are built. The test framework builds several containers
/// (one main and a pool for parallel tests) by calling <see cref="BuildServiceProvider"/> and hands a free one to each test.
/// Select the environment with <c>[assembly: AnchTestFramework&lt;TEnvironment&gt;]</c>.
/// </summary>
public interface ITestEnvironment
{
    /// <summary>Key under which services that must run only in the main container (for example database snapshot creation) are registered.</summary>
    public const string MainServiceProviderKey = "ServiceProviderPool.Main";

    /// <summary>Registers the services of the environment and builds the container.</summary>
    /// <param name="services">The collection to register in; already contains the infrastructure services.</param>
    /// <param name="buildContext">Says which container is being built (the main one or a pooled one).</param>
    IServiceProvider BuildServiceProvider(IServiceCollection services, ServiceProviderBuildContext buildContext);
}