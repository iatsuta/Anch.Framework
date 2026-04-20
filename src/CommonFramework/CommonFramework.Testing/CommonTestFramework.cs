using System.Collections.Concurrent;
using System.Reflection;

using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonTestFramework : XunitTestFramework
{
    private readonly ConcurrentDictionary<Assembly, IServiceProvider> rootServiceProviderCache = [];

    private IServiceProvider GetRootServiceProvider(Assembly assembly)
    {
        return this.rootServiceProviderCache.GetOrAdd(assembly, asm =>
        {
            var commonTestFrameworkAttribute = asm.GetCustomAttribute<CommonTestFrameworkAttribute>()
                                               ?? throw new InvalidOperationException(
                                                   $"Assembly '{asm.FullName}' must be decorated with '{typeof(CommonTestFrameworkAttribute).FullName}' attribute.");

            var initializer = (ICommonTestFrameworkInitializer)Activator.CreateInstance(commonTestFrameworkAttribute.InitializerType)!;

            return initializer.BuildServiceProvider(initializer.CreateServiceCollection());
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