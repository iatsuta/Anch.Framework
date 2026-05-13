namespace Anch.Testing;

public record PooledServiceProviderBuildContext(ServiceProviderIndex Index, IServiceProvider MainServiceProvider) : ServiceProviderBuildContext(Index);