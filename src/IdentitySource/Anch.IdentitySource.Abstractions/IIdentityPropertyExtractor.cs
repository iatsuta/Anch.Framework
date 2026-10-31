using System.Reflection;

namespace Anch.IdentitySource;

/// <summary>Locates the identifier property of a type by convention.</summary>
public interface IIdentityPropertyExtractor
{
    /// <summary>Finds the identifier property.</summary>
    /// <param name="domainType">The domain type.</param>
    /// <returns>The property, or <c>null</c> if the type has none.</returns>
    PropertyInfo? TryExtract(Type domainType);
}