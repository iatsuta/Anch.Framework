using System.Reflection;

using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class XUnitServiceProviderPoolFactory : IServiceProviderPoolFactory
{
    public IServiceProviderPool Create(Assembly assembly, ITestEnvironment testEnvironment)
    {
        var parallelizationAttribute = assembly.GetCustomAttribute<ParallelizationAttribute>();

        return new ServiceProviderPool(testEnvironment, parallelizationAttribute?.GetMode() != ParallelMode.None);
    }
}