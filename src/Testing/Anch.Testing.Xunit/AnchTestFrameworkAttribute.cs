using Anch.Testing.Xunit.Engine;

using Xunit.v3;

namespace Anch.Testing.Xunit;

[AttributeUsage(AttributeTargets.Assembly)]
public class AnchTestFrameworkAttribute(Type? testEnvironmentType) : Attribute, ITestFrameworkAttribute
{
    public AnchTestFrameworkAttribute()
        : this(null)
    {
    }

    public Type FrameworkType { get; } = typeof(AnchTestFramework);

    public Type? TestEnvironmentType { get; } = testEnvironmentType;

    public Type ServiceProviderPoolFactoryType { get; init; } = typeof(XUnitServiceProviderPoolFactory);
}

public class AnchTestFrameworkAttribute<TTestEnvironment>() : AnchTestFrameworkAttribute(typeof(TTestEnvironment))
    where TTestEnvironment : ITestEnvironment, new();