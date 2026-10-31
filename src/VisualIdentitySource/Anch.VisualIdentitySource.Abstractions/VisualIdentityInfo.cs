using System.Linq.Expressions;

using Anch.Core;

namespace Anch.VisualIdentitySource;

/// <summary>The name property of <typeparamref name="TDomainObject"/>.</summary>
/// <param name="Name">The name property.</param>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public record VisualIdentityInfo<TDomainObject>(PropertyAccessors<TDomainObject, string> Name) : VisualIdentityInfo, IVisualIdentityInfo<TDomainObject>
{
    /// <summary>Creates the description from a property expression.</summary>
    /// <param name="namePath">Selects the name property.</param>
    public VisualIdentityInfo(Expression<Func<TDomainObject, string>> namePath) :
        this(namePath.ToPropertyAccessors())
    {
    }

    public override Type DomainObjectType { get; } = typeof(TDomainObject);
}

/// <summary>Untyped description of the name property of a domain type.</summary>
public abstract record VisualIdentityInfo : IVisualIdentityInfo
{
    public abstract Type DomainObjectType { get; }
}