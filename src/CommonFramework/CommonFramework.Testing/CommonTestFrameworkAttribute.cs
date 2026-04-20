using Xunit.v3;

namespace CommonFramework.Testing;

[AttributeUsage(AttributeTargets.Assembly)]
public abstract class CommonTestFrameworkAttribute : Attribute, ITestFrameworkAttribute
{
    public Type FrameworkType { get; } = typeof(CommonTestFramework);

    public abstract Type InitializerType { get; }
}

public class CommonTestFrameworkAttribute<TInitializer> : CommonTestFrameworkAttribute
    where TInitializer : ICommonTestFrameworkInitializer
{
    public override Type InitializerType { get; } = typeof(TInitializer);
}