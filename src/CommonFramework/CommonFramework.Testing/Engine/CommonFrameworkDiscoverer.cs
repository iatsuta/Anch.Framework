using System.Reflection;

using Xunit.Sdk;
using Xunit.v3;

namespace CommonFramework.Testing.Engine;

public class CommonFrameworkDiscoverer(IXunitTestAssembly testAssembly, IServiceProvider rootServiceProvider)
    : XunitTestFrameworkDiscoverer(testAssembly)
{
    protected override ValueTask<bool> FindTestsForMethod(IXunitTestMethod testMethod, ITestFrameworkDiscoveryOptions discoveryOptions,
        Func<ITestCase, ValueTask<bool>> discoveryCallback)
    {
        var actualTestMethod = testMethod.Method.GetCustomAttributes<CommonMemberDataAttribute>().Any() ? new CommonTheoryTestMethod(testMethod, rootServiceProvider) : testMethod;

        return base.FindTestsForMethod(actualTestMethod, discoveryOptions, discoveryCallback);
    }
}