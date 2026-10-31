using System.Linq.Expressions;

using Anch.Core;

namespace Anch.IdentitySource;

/// <summary>Injectable identifier description of a type with a known identifier type; resolved through the identity source.</summary>
/// <param name="identityInfoSource">Finds the description.</param>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
/// <typeparam name="TIdent">The type of the identifier.</typeparam>
public class IdentityInfoProxy<TDomainObject, TIdent>(IIdentityInfoSource identityInfoSource)
    : IdentityInfoProxyBase<TDomainObject>, IIdentityInfo<TDomainObject, TIdent>
    where TIdent : notnull
{
    private readonly IdentityInfo<TDomainObject, TIdent> innerInfo = identityInfoSource.GetIdentityInfo<TDomainObject, TIdent>();

    /// <inheritdoc/>
    protected override IdentityInfo<TDomainObject> InnerInfo => this.innerInfo;

    /// <inheritdoc/>
    public PropertyAccessors<TDomainObject, TIdent> Id => this.innerInfo.Id;

    /// <inheritdoc/>
    public Expression<Func<TDomainObject, bool>> CreateFilter(IEnumerable<TIdent> idents) => this.innerInfo.CreateFilter(idents);

    /// <inheritdoc/>
    public Expression<Func<TDomainObject, bool>> CreateFilter(TIdent ident) => this.innerInfo.CreateFilter(ident);
}

/// <summary>Injectable identifier description of a type; resolved through the identity source.</summary>
/// <param name="identityInfoSource">Finds the description.</param>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public class IdentityInfoProxy<TDomainObject>(IIdentityInfoSource identityInfoSource) : IdentityInfoProxyBase<TDomainObject>
{
    /// <inheritdoc/>
    protected override IdentityInfo<TDomainObject> InnerInfo { get; } = identityInfoSource.GetIdentityInfo<TDomainObject>();
}

/// <summary>The part shared by the identity description proxies.</summary>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public abstract class IdentityInfoProxyBase<TDomainObject> : IIdentityInfo<TDomainObject>
{
    /// <summary>The description the proxy delegates to.</summary>
    protected abstract IdentityInfo<TDomainObject> InnerInfo { get; }

    /// <inheritdoc/>
    public Type DomainObjectType => this.InnerInfo.DomainObjectType;

    /// <inheritdoc/>
    public Type IdentityType => this.InnerInfo.IdentityType;

    /// <inheritdoc/>
    public object GetId(TDomainObject domainObject) => this.InnerInfo.GetId(domainObject);
}