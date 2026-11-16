using System.Collections.Concurrent;
using System.Reflection;

using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

/// <summary>The xunit test framework that runs tests with containers from a pool built by a test environment.</summary>
public class AnchTestFramework : XunitTestFramework
{
    private readonly ConcurrentDictionary<Assembly, IServiceProviderPool?> serviceProviderPoolCache = [];

    private readonly ConcurrentDictionary<Assembly, ITestFrameworkExecutor> executorCache = [];

    private readonly ConcurrentDictionary<Assembly, ITestFrameworkDiscoverer> discovererCache = [];

    private IServiceProviderPool? GetServiceProviderPool(Assembly assembly) =>

        this.serviceProviderPoolCache.GetOrAdd(assembly,
            _ => assembly.TryCreateTestEnvironment() is var (testEnvironment, poolFactory) ? poolFactory.Create(assembly, testEnvironment) : null);

    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly) =>
        this.executorCache.GetOrAdd(assembly,
            _ =>
            {
                var serviceProviderPool = this.GetServiceProviderPool(assembly);

                var rootRunner =
                    new AnchTestAssemblyRunner(
                        new AnchTestCollectionRunner(
                            new AnchTestClassRunner(serviceProviderPool)));

                return new AnchFrameworkExecutor(new XunitTestAssembly(assembly, null), rootRunner, serviceProviderPool);
            });

    protected override ITestFrameworkDiscoverer CreateDiscoverer(Assembly assembly) =>
        this.discovererCache.GetOrAdd(assembly,
            _ =>
                new AnchFrameworkDiscoverer(new XunitTestAssembly(assembly, null),
                    this.GetServiceProviderPool(assembly)));
}