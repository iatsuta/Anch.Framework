using System.Reflection;

using Xunit.Sdk;
using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonFrameworkDiscoverer(IXunitTestAssembly testAssembly, IServiceProvider serviceProvider)
    : XunitTestFrameworkDiscoverer(testAssembly)
{
    protected override ValueTask<bool> FindTestsForMethod(IXunitTestMethod testMethod, ITestFrameworkDiscoveryOptions discoveryOptions,
        Func<ITestCase, ValueTask<bool>> discoveryCallback)
    {
        var actualTestMethod = testMethod.Method.GetCustomAttributes<CommonMemberDataAttribute>().Any() ? new CommonTheoryTestMethod(testMethod, serviceProvider) : testMethod;

        return base.FindTestsForMethod(actualTestMethod, discoveryOptions, discoveryCallback);
    }
}