namespace Anch.Testing;

/// <summary>The position of a container in the pool. The main container has the special index <see cref="Main"/>.</summary>
/// <param name="Index">The position, or -1 for the main container.</param>
public record ServiceProviderIndex(int Index)
{
    /// <summary><c>true</c> for the main container.</summary>
    public bool IsMain => this == Main;

    /// <summary>The index of the main container.</summary>
    public static ServiceProviderIndex Main { get; } = new(-1);
}