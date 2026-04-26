using System.Collections.Concurrent;
using System.Reflection;

using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestFramework : XunitTestFramework
{
    private readonly ConcurrentDictionary<Assembly, ITestEnvironment?> testEnvironmentCache = [];

    private readonly ConcurrentDictionary<Assembly, IServiceProviderPool?> rootServiceProviderPoolCache = [];

    private ITestEnvironment? GetTestEnvironment(Assembly assembly)
    {
        return this.testEnvironmentCache.GetOrAdd(assembly, asm =>
        {
            var commonTestFrameworkAttribute = asm.GetCustomAttribute<AnchTestFrameworkAttribute>()
                                               ?? throw new InvalidOperationException(
                                                   $"Assembly '{asm.FullName}' must be decorated with '{typeof(AnchTestFrameworkAttribute).FullName}' attribute");

            return commonTestFrameworkAttribute.TestEnvironmentType == null
                ? null
                : Activator.CreateInstance(commonTestFrameworkAttribute.TestEnvironmentType) as ITestEnvironment
                  ?? throw new InvalidOperationException(
                      $"Failed to create initializer of type '{commonTestFrameworkAttribute.TestEnvironmentType.FullName}'");
        });
    }

    private IServiceProviderPool? GetServiceProviderPool(Assembly assembly) =>

        this.rootServiceProviderPoolCache.GetOrAdd(assembly,
            _ => this.GetTestEnvironment(assembly) is { } testEnvironment
                ? new ServiceProviderPool(testEnvironment)
                : null);

    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly)
    {
        var rootRunner =
            new AnchTestAssemblyRunner(
                new AnchTestCollectionRunner(
                    new AnchTestClassRunner(), this.GetServiceProviderPool(assembly)));

        return new AnchFrameworkExecutor(new XunitTestAssembly(assembly), rootRunner);
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer(Assembly assembly) =>

        new AnchFrameworkDiscoverer(new XunitTestAssembly(assembly, null, assembly.GetName().Version),
            this.GetServiceProviderPool(assembly));
}