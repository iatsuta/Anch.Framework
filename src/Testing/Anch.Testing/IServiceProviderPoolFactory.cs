using System.Reflection;

namespace Anch.Testing;

public interface IServiceProviderPoolFactory
{
    IServiceProviderPool Create(Assembly assembly, ITestEnvironment testEnvironment);
}