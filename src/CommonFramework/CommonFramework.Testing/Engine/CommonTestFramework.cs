using System.Collections.Concurrent;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Xunit.v3;

namespace CommonFramework.Testing.Engine;

public class CommonTestFramework : XunitTestFramework
{
    private readonly ConcurrentDictionary<Assembly, ITestServiceProviderBuilder> initializerCache = [];

    private readonly ConcurrentDictionary<Assembly, IServiceProvider> rootServiceProviderCache = [];

    private ITestServiceProviderBuilder GetServiceProviderBuilder(Assembly assembly)
    {
        return this.initializerCache.GetOrAdd(assembly, asm =>
        {
            var commonTestFrameworkAttribute = asm.GetCustomAttribute<CommonTestFrameworkAttribute>()
                                               ?? throw new InvalidOperationException(
                                                   $"Assembly '{asm.FullName}' must be decorated with '{typeof(CommonTestFrameworkAttribute).FullName}' attribute");

            return Activator.CreateInstance(commonTestFrameworkAttribute.ServiceProviderBuilderType) as ITestServiceProviderBuilder
                   ?? throw new InvalidOperationException(
                       $"Failed to create initializer of type '{commonTestFrameworkAttribute.ServiceProviderBuilderType.FullName}'");
        });
    }

    private IServiceProvider GetRootServiceProvider(Assembly assembly)
    {
        return this.rootServiceProviderCache.GetOrAdd(assembly, _ =>
        {
            var services = new ServiceCollection();

            return this.GetServiceProviderBuilder(assembly).Build(services);
        });
    }

    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly)
    {
        var rootServiceProvider = this.GetRootServiceProvider(assembly);

        var rootRunner = new CommonXunitTestAssemblyRunner(new CommonTestCollectionRunner(new CommonTestClassRunner(rootServiceProvider)));

        return new CommonFrameworkExecutor(new XunitTestAssembly(assembly), rootRunner);
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer(Assembly assembly)
    {
        return new CommonFrameworkDiscoverer(new XunitTestAssembly(assembly, null, assembly.GetName().Version), this.GetRootServiceProvider(assembly));
    }
}