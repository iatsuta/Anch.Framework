using Anch.GenericQueryable.DependencyInjection;

namespace Anch.GenericQueryable.EntityFramework;

/// <summary>Configures GenericQueryable for an EF Core context.</summary>
public interface IEfGenericQueryableExtensionInnerSetup
{
    /// <summary>Applies the configuration.</summary>
    /// <param name="setup">The GenericQueryable setup.</param>
    void Initialize(IGenericQueryableSetup setup);
}