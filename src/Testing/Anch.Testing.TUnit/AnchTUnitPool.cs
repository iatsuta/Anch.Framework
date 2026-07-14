namespace Anch.Testing.TUnit;

public static class AnchTUnitPool<TEnvironment>
    where TEnvironment : ITestEnvironment, new()
{
    public static ServiceProviderPool Instance { get; } = new ServiceProviderPool(new TEnvironment(), true);
}