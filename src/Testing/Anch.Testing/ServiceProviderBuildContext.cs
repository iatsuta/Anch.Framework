namespace Anch.Testing;

public record ServiceProviderBuildContext(ServiceProviderIndex Index)
{
    public static ServiceProviderBuildContext Main { get; } = new (ServiceProviderIndex.Main);
}