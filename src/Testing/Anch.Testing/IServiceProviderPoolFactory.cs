using System.Reflection;

namespace Anch.Testing;

/// <summary>Creates the pool of service containers for a test assembly.</summary>
public interface IServiceProviderPoolFactory
{
    IServiceProviderPool Create(Assembly assembly, ITestEnvironment testEnvironment);
}