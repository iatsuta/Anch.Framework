using Anch.Testing.Xunit.Engine;

using Xunit.v3;

namespace Anch.Testing.Xunit;

/// <summary>Assembly attribute that switches the assembly to the Anch test framework and selects the test environment.</summary>
/// <param name="testEnvironmentType">The <see cref="ITestEnvironment"/> type, or <c>null</c> for no environment.</param>
[AttributeUsage(AttributeTargets.Assembly)]
public class AnchTestFrameworkAttribute(Type? testEnvironmentType) : Attribute, ITestFrameworkAttribute
{
    /// <summary>Uses the Anch test framework without a test environment.</summary>
    public AnchTestFrameworkAttribute()
        : this(null)
    {
    }

    /// <summary>The xunit test framework type.</summary>
    public Type FrameworkType { get; } = typeof(AnchTestFramework);

    /// <summary>The <see cref="ITestEnvironment"/> type.</summary>
    public Type? TestEnvironmentType { get; } = testEnvironmentType;

    /// <summary>The type that creates the pool of containers; replace to customize the pool.</summary>
    public Type ServiceProviderPoolFactoryType { get; init; } = typeof(XUnitServiceProviderPoolFactory);
}

/// <summary>Assembly attribute that switches the assembly to the Anch test framework with the given test environment.</summary>
/// <typeparam name="TTestEnvironment">The environment type; it needs a parameterless constructor.</typeparam>
public class AnchTestFrameworkAttribute<TTestEnvironment>() : AnchTestFrameworkAttribute(typeof(TTestEnvironment))
    where TTestEnvironment : ITestEnvironment, new();