using System.Reflection;

namespace Anch.VisualIdentitySource;

/// <summary>Locates the name property of a type by convention.</summary>
public interface IVisualIdentityPropertyExtractor
{
    /// <summary>Finds the name property.</summary>
    /// <param name="domainType">The domain type.</param>
    /// <returns>The property, or <c>null</c> if the type has none.</returns>
    PropertyInfo? TryExtract(Type domainType);
}