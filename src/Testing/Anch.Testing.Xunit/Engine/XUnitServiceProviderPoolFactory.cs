using System.Reflection;

using Xunit;

namespace Anch.Testing.Xunit.Engine;

public class XUnitServiceProviderPoolFactory : IServiceProviderPoolFactory
{
    public IServiceProviderPool Create(Assembly assembly, ITestEnvironment testEnvironment)
    {
        var collectionBehaviorAttribute = assembly.GetCustomAttribute<CollectionBehaviorAttribute>();

        return new ServiceProviderPool(testEnvironment, !collectionBehaviorAttribute?.DisableTestParallelization);
    }
}