namespace Anch.Testing;

/// <summary>Says which container <c>ITestEnvironment.BuildServiceProvider</c> is building.</summary>
/// <param name="Index">The position of the container in the pool, or <see cref="ServiceProviderIndex.Main"/>.</param>
public record ServiceProviderBuildContext(ServiceProviderIndex Index)
{
    /// <summary>The context of the main container.</summary>
    public static ServiceProviderBuildContext Main { get; } = new(ServiceProviderIndex.Main);
}