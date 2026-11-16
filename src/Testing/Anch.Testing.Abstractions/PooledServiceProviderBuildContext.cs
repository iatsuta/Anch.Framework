namespace Anch.Testing;

/// <summary>The context of a pooled container: its position in the pool and the main container.</summary>
/// <param name="Index">The position of the container in the pool.</param>
/// <param name="MainServiceProvider">The main container.</param>
public record PooledServiceProviderBuildContext(ServiceProviderIndex Index, IServiceProvider MainServiceProvider) : ServiceProviderBuildContext(Index);