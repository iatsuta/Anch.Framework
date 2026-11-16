namespace Anch.Testing.Database.ConnectionStringManagement;

/// <summary>The role of the private database of one pooled container.</summary>
public record PoolTestConnectionStringRole(ServiceProviderIndex ServiceProviderIndex) : TestConnectionStringRole("Actual")
{
    public static PoolTestConnectionStringRole Main { get; } = new(ServiceProviderIndex.Main);
}