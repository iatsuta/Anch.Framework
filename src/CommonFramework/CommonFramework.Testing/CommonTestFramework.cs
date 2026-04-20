using System.Collections.Concurrent;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonTestFramework : XunitTestFramework
{
    private readonly ConcurrentDictionary<Assembly, IServiceProvider> cache = [];

    private IServiceProvider GetRootServiceProvider(Assembly assembly)
    {
        return this.cache.GetOrAdd(assembly, asm =>
        {
            var commonTestFrameworkAttribute = asm.GetCustomAttribute<CommonTestFrameworkAttribute>()
                                               ?? throw new InvalidOperationException(
                                                   $"Assembly '{asm.FullName}' must be decorated with '{typeof(CommonTestFrameworkAttribute).FullName}' attribute.");

            var initializer = (ICommonTestFrameworkInitializer)Activator.CreateInstance(commonTestFrameworkAttribute.InitializerType)!;

            return initializer.BuildServiceProvider(new ServiceCollection());
        });
    }

    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly)
    {
        return new CommonFrameworkExecutor(new XunitTestAssembly(assembly), this.GetRootServiceProvider(assembly));
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer(Assembly assembly)
    {
        return new CommonFrameworkDiscoverer(new XunitTestAssembly(assembly, null, assembly.GetName().Version), this.GetRootServiceProvider(assembly));
    }
}