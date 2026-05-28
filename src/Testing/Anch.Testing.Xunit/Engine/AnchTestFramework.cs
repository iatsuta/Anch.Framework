using System.Collections.Concurrent;
using System.Reflection;

using Xunit;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestFramework : XunitTestFramework
{
    private readonly ConcurrentDictionary<Assembly, IServiceProviderPool?> serviceProviderPoolCache = [];

    private readonly ConcurrentDictionary<Assembly, ITestFrameworkExecutor> executorCache = [];

    private readonly ConcurrentDictionary<Assembly, ITestFrameworkDiscoverer> discovererCache = [];

    private IServiceProviderPool? GetServiceProviderPool(Assembly assembly) =>

        this.serviceProviderPoolCache.GetOrAdd(assembly,
            _ =>
            {
                var collectionBehaviorAttribute = assembly.GetCustomAttribute<CollectionBehaviorAttribute>();

                return assembly.TryCreateTestEnvironment() is { } testEnvironment
                    ? new ServiceProviderPool(testEnvironment, !collectionBehaviorAttribute?.DisableTestParallelization)
                    : null;
            });

    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly) =>
        this.executorCache.GetOrAdd(assembly,
            _ =>
            {
                var serviceProviderPool = this.GetServiceProviderPool(assembly);

                var rootRunner =
                    new AnchTestAssemblyRunner(
                        new AnchTestCollectionRunner(
                            new AnchTestClassRunner(serviceProviderPool)));

                return new AnchFrameworkExecutor(new XunitTestAssembly(assembly), rootRunner, serviceProviderPool, this.CreateDiscoverer(assembly));
            });

    protected override ITestFrameworkDiscoverer CreateDiscoverer(Assembly assembly) =>
        this.discovererCache.GetOrAdd(assembly,
            _ =>
                new AnchFrameworkDiscoverer(new XunitTestAssembly(assembly, null, assembly.GetName().Version),
                    this.GetServiceProviderPool(assembly)));
}