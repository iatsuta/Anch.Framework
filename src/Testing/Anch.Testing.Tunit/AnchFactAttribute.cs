using Anch.Testing.Xunit.Engine;

using Xunit;
using Xunit.v3;

namespace Anch.Testing.Xunit;

[assembly: AnchTUnit<TestEnvironment>]

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
public sealed class AnchTUnitAttribute<TEnvironment> : ClassConstructorAttribute
    where TEnvironment : ITestEnvironment, new()
{
    public AnchTUnitAttribute()
        : base(typeof(AnchTUnitClassConstructor<TEnvironment>))
    {
    }
}
internal static class AnchTUnitPool<TEnvironment>
    where TEnvironment : ITestEnvironment, new()
{
    public static IServiceProviderPool Instance => new ServiceProviderPool();

    public static async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1 || !Pool.IsValueCreated)
        {
            return;
        }

        await Pool.Value.DisposeAsync();
    }
}