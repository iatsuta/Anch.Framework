using System.Linq.Expressions;

using Anch.Core;

namespace Anch.IdentitySource;

/// <summary>The identifier property of <typeparamref name="TDomainObject"/>.</summary>
/// <param name="Id">The identifier property.</param>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
/// <typeparam name="TIdent">The type of the identifier.</typeparam>
public record IdentityInfo<TDomainObject, TIdent>(PropertyAccessors<TDomainObject, TIdent> Id) : IdentityInfo<TDomainObject>, IIdentityInfo<TDomainObject, TIdent>
    where TIdent : notnull
{
    /// <summary>Creates the description from a property expression.</summary>
    /// <param name="idPath">Selects the identifier property.</param>
    public IdentityInfo(Expression<Func<TDomainObject, TIdent>> idPath) :
        this(idPath.ToPropertyAccessors())
    {
    }

    public override Type IdentityType { get; } = typeof(TIdent);

    /// <inheritdoc/>
    public Expression<Func<TDomainObject, bool>> CreateFilter(IEnumerable<TIdent> idents)
    {
        return this.Id.Path.Select(ident => idents.Contains(ident));
    }

    /// <inheritdoc/>
    public Expression<Func<TDomainObject, bool>> CreateFilter(TIdent ident)
    {
        return this.Id.Path.Select(ExpressionHelper.GetEqualityWithExpr(ident));
    }

    /// <inheritdoc/>
    public override object GetId(TDomainObject domainObject)
    {
        return this.Id.Getter(domainObject);
    }
}

/// <summary>The identifier of <typeparamref name="TDomainObject"/> without the identifier type.</summary>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public abstract record IdentityInfo<TDomainObject> : IdentityInfo, IIdentityInfo<TDomainObject>
{
    public override Type DomainObjectType { get; } = typeof(TDomainObject);

    public abstract object GetId(TDomainObject domainObject);
}

/// <summary>Untyped description of the identifier of a domain type.</summary>
public abstract record IdentityInfo
{
    public abstract Type DomainObjectType { get; }

    public abstract Type IdentityType { get; }
}