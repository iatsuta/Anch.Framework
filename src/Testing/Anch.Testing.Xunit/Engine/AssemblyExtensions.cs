using System.Reflection;

namespace Anch.Testing.Xunit.Engine;

public static class AssemblyExtensions
{
    public static (ITestEnvironment, IServiceProviderPoolFactory)? TryCreateTestEnvironment(this Assembly assembly)
    {
        var commonTestFrameworkAttribute = assembly.GetCustomAttribute<AnchTestFrameworkAttribute>()
                                           ?? throw new InvalidOperationException(
                                               $"Assembly '{assembly.FullName}' must be decorated with '{typeof(AnchTestFrameworkAttribute).FullName}' attribute");

        return commonTestFrameworkAttribute.TestEnvironmentType == null
            ? null
            : (Activator.CreateInstance(commonTestFrameworkAttribute.TestEnvironmentType) as ITestEnvironment
               ?? throw new InvalidOperationException(
                   $"Failed to create initializer of type '{commonTestFrameworkAttribute.TestEnvironmentType.FullName}'"),
                Activator.CreateInstance(commonTestFrameworkAttribute.ServiceProviderPoolFactoryType) as IServiceProviderPoolFactory
                ?? throw new InvalidOperationException(
                    $"Failed to create service provider pool factory of type '{commonTestFrameworkAttribute.ServiceProviderPoolFactoryType.FullName}'"));
    }
}