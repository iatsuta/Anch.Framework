using Anch.Core;

namespace Anch.VisualIdentitySource;

/// <summary>Describes the name property of a domain type.</summary>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public interface IVisualIdentityInfo<TDomainObject> : IVisualIdentityInfo
{
    /// <summary>The name property (path expression plus compiled getter and setter).</summary>
    PropertyAccessors<TDomainObject, string> Name { get; }
}

/// <summary>Untyped description of the name property of a domain type.</summary>
public interface IVisualIdentityInfo
{
    /// <summary>The domain type.</summary>
    Type DomainObjectType { get; }
}
