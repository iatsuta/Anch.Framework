using CommonFramework.Testing.XunitEngine;
using Xunit.v3;

namespace CommonFramework.Testing;

[AttributeUsage(AttributeTargets.Assembly)]
public class CommonTestFrameworkAttribute : Attribute, ITestFrameworkAttribute
{
    public Type FrameworkType { get; } = typeof(CommonTestFramework);

    public virtual Type? ServiceProviderBuilderType { get; } = null;
}

public class CommonTestFrameworkAttribute<TServiceProviderBuilder> : CommonTestFrameworkAttribute
    where TServiceProviderBuilder : ITestServiceProviderBuilder
{
    public override Type ServiceProviderBuilderType { get; } = typeof(TServiceProviderBuilder);
}