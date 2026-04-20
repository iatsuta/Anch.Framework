using CommonFramework.Testing.Engine;

using Xunit.v3;

namespace CommonFramework.Testing;

[AttributeUsage(AttributeTargets.Assembly)]
public abstract class CommonTestFrameworkAttribute : Attribute, ITestFrameworkAttribute
{
    public Type FrameworkType { get; } = typeof(CommonTestFramework);

    public abstract Type ServiceProviderBuilderType { get; }
}

public class CommonTestFrameworkAttribute<TServiceProviderBuilder> : CommonTestFrameworkAttribute
    where TServiceProviderBuilder : ITestServiceProviderBuilder
{
    public override Type ServiceProviderBuilderType { get; } = typeof(TServiceProviderBuilder);
}