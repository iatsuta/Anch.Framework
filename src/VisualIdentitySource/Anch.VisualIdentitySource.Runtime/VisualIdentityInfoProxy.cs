using Anch.Core;

namespace Anch.VisualIdentitySource;

/// <summary>Injectable name description of a type; resolved through the visual identity source.</summary>
/// <param name="visualIdentityInfoSource">Finds the description.</param>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public class VisualIdentityInfoProxy<TDomainObject>(IVisualIdentityInfoSource visualIdentityInfoSource) : IVisualIdentityInfo<TDomainObject>
{
    private readonly VisualIdentityInfo<TDomainObject> innerInfo = visualIdentityInfoSource.GetVisualIdentityInfo<TDomainObject>();

    /// <inheritdoc/>
    public Type DomainObjectType => this.innerInfo.DomainObjectType;

    /// <inheritdoc/>
    public PropertyAccessors<TDomainObject, string> Name => this.innerInfo.Name;
}