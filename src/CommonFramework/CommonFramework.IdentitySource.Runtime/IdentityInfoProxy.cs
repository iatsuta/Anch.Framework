using System.Linq.Expressions;

namespace CommonFramework.IdentitySource;

public class IdentityInfoProxy<TDomainObject, TIdent>(IIdentityInfoSource identityInfoSource)
    : IdentityInfoProxyBase<TDomainObject>, IIdentityInfo<TDomainObject, TIdent>
    where TIdent : notnull
{
    private readonly IdentityInfo<TDomainObject, TIdent> innerInfo = identityInfoSource.GetIdentityInfo<TDomainObject, TIdent>();

    protected override IdentityInfo<TDomainObject> InnerInfo => this.innerInfo;

    public PropertyAccessors<TDomainObject, TIdent> Id => this.innerInfo.Id;

    public Expression<Func<TDomainObject, bool>> CreateFilter(IEnumerable<TIdent> idents) => this.innerInfo.CreateFilter(idents);

    public Expression<Func<TDomainObject, bool>> CreateFilter(TIdent ident) => this.innerInfo.CreateFilter(ident);
}

public class IdentityInfoProxy<TDomainObject>(IIdentityInfoSource identityInfoSource) : IdentityInfoProxyBase<TDomainObject>
{
    protected override IdentityInfo<TDomainObject> InnerInfo { get; } = identityInfoSource.GetIdentityInfo<TDomainObject>();
}

public abstract class IdentityInfoProxyBase<TDomainObject> : IIdentityInfo<TDomainObject>
{
    protected abstract IdentityInfo<TDomainObject> InnerInfo { get; }

    public Type DomainObjectType => this.InnerInfo.DomainObjectType;

    public Type IdentityType => this.InnerInfo.IdentityType;

    public object GetId(TDomainObject domainObject) => this.InnerInfo.GetId(domainObject);
}