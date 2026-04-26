using Anch.Core;

namespace Anch.VisualIdentitySource;

public interface IVisualIdentityInfo<TDomainObject> : IVisualIdentityInfo
{
    PropertyAccessors<TDomainObject, string> Name { get; }
}

public interface IVisualIdentityInfo
{
    Type DomainObjectType { get; }
}
