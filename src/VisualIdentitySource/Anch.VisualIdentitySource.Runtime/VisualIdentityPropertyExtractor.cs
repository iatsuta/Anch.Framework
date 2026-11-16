using System.Reflection;

namespace Anch.VisualIdentitySource;

/// <summary>Finds the name as the first public <see cref="string"/> property whose name is in the configured list.</summary>
/// <param name="settings">The convention settings.</param>
public class VisualIdentityPropertyExtractor(VisualIdentityPropertySourceSettings settings) : IVisualIdentityPropertyExtractor
{
    /// <inheritdoc/>
    public PropertyInfo? TryExtract(Type domainType)
    {
        var request =

            from propertyName in settings.PropertyNameList

            let property = domainType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)

            where property is not null && property.PropertyType == typeof(string)

            select property;

        return request.FirstOrDefault();
    }
}