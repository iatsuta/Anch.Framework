using System.Reflection;

using Xunit;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public static class XunitTestMethodExtensions
{
    public static IXunitTestCase WithServiceProviderPool(this IXunitTestCase testCase, IServiceProviderPool? serviceProviderPool)
    {
        var actualTestMethod = testCase.TestMethod.WithServiceProviderPool(serviceProviderPool);

        if (testCase.TestMethod == actualTestMethod)
        {
            return testCase;
        }
        else
        {
            return new AnchTestCase(testCase, actualTestMethod);
        }
    }

    public static IXunitTestMethod WithServiceProviderPool(this IXunitTestMethod testMethod, IServiceProviderPool? serviceProviderPool)
    {
        if (testMethod is AnchTheoryTestMethod anchTheoryTestMethod)
        {
            anchTheoryTestMethod.ServiceProviderPool = serviceProviderPool;

            return anchTheoryTestMethod;
        }
        else if (testMethod.Method.GetCustomAttributes<TheoryAttribute>().Any())
        {
            return new AnchTheoryTestMethod(testMethod) { ServiceProviderPool = serviceProviderPool };
        }
        else
        {
            return testMethod;
        }
    }
}