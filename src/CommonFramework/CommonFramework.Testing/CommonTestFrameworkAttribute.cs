using CommonFramework.Testing.XunitEngine;

using Xunit.v3;

namespace CommonFramework.Testing;

[AttributeUsage(AttributeTargets.Assembly)]
public class CommonTestFrameworkAttribute : Attribute, ITestFrameworkAttribute
{
    public Type FrameworkType { get; } = typeof(CommonTestFramework);

    public virtual Type? TestEnvironmentType { get; } = null;
}

public class CommonTestFrameworkAttribute<TTestEnvironment> : CommonTestFrameworkAttribute
    where TTestEnvironment : ITestEnvironment
{
    public override Type TestEnvironmentType { get; } = typeof(TTestEnvironment);
}