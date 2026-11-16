using System.Reflection;

namespace Anch.IdentitySource;

/// <summary>Finds the identifier as a public instance property with the configured name.</summary>
/// <param name="settings">The convention settings.</param>
public class IdentityPropertyExtractor(IdentityPropertySourceSettings settings) : IIdentityPropertyExtractor
{
    /// <inheritdoc/>
    public PropertyInfo? TryExtract(Type domainType)
    {
        return domainType.GetProperty(settings.PropertyName, BindingFlags.Public | BindingFlags.Instance);
    }
}