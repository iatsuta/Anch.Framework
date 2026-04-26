using Anch.Core;

namespace Anch.VisualIdentitySource;

public class VisualIdentityInfoProxy<TDomainObject>(IVisualIdentityInfoSource visualIdentityInfoSource) : IVisualIdentityInfo<TDomainObject>
{
    private readonly VisualIdentityInfo<TDomainObject> innerInfo = visualIdentityInfoSource.GetVisualIdentityInfo<TDomainObject>();

    public Type DomainObjectType => this.innerInfo.DomainObjectType;

    public PropertyAccessors<TDomainObject, string> Name => this.innerInfo.Name;
}