using System.Linq.Expressions;
using Anch.Core;

namespace Anch.IdentitySource;

public record IdentityInfo<TDomainObject, TIdent>(PropertyAccessors<TDomainObject, TIdent> Id) : IdentityInfo<TDomainObject>, IIdentityInfo<TDomainObject, TIdent>
    where TIdent : notnull
{
	public IdentityInfo(Expression<Func<TDomainObject, TIdent>> idPath) :
		this(idPath.ToPropertyAccessors())
	{
	}

	public override Type IdentityType { get; } = typeof(TIdent);

    public Expression<Func<TDomainObject, bool>> CreateFilter(IEnumerable<TIdent> idents)
    {
        return this.Id.Path.Select(ident => idents.Contains(ident));
    }

    public Expression<Func<TDomainObject, bool>> CreateFilter(TIdent ident)
    {
        return this.Id.Path.Select(ExpressionHelper.GetEqualityWithExpr(ident));
    }

    public override object GetId(TDomainObject domainObject)
    {
	    return this.Id.Getter(domainObject);
    }
}

public abstract record IdentityInfo<TDomainObject> : IdentityInfo, IIdentityInfo<TDomainObject>
{
    public override Type DomainObjectType { get; } = typeof(TDomainObject);

    public abstract object GetId(TDomainObject domainObject);
}

public abstract record IdentityInfo
{
    public abstract Type DomainObjectType { get; }

    public abstract Type IdentityType { get; }
}