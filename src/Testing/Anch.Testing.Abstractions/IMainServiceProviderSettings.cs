namespace Anch.Testing;

/// <summary>Settings of the main service provider.</summary>
public interface IMainServiceProviderSettings
{
    /// <summary>Whether the main service provider is returned to the pool after initialization.</summary>
    bool ReturnToPool { get; }
}