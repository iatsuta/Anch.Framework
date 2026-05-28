using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchFrameworkDiscoverer(IXunitTestAssembly testAssembly, IServiceProviderPool? serviceProviderPool)
    : XunitTestFrameworkDiscoverer(testAssembly)
{
    protected override ValueTask<bool> FindTestsForMethod(IXunitTestMethod testMethod, ITestFrameworkDiscoveryOptions discoveryOptions,
        Func<ITestCase, ValueTask<bool>> discoveryCallback)
    {
        var actualTestMethod = testMethod.WithServiceProviderPool(serviceProviderPool);

        return base.FindTestsForMethod(actualTestMethod, discoveryOptions, discoveryCallback);
    }
}