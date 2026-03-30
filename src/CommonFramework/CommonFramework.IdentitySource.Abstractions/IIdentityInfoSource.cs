namespace CommonFramework.IdentitySource;

public interface IIdentityInfoSource
{
    IdentityInfo GetIdentityInfo(Type domainType);

    IdentityInfo<TDomainObject> GetIdentityInfo<TDomainObject>()
        => (IdentityInfo<TDomainObject>)this.GetIdentityInfo(typeof(TDomainObject));

    IdentityInfo<TDomainObject, TIdent> GetIdentityInfo<TDomainObject, TIdent>()
        where TIdent : notnull => (IdentityInfo<TDomainObject, TIdent>)this.GetIdentityInfo(typeof(TDomainObject));

    IdentityInfo? TryGetIdentityInfo(Type domainType);

    IdentityInfo<TDomainObject>? TryGetIdentityInfo<TDomainObject>()
        => (IdentityInfo<TDomainObject>?)this.TryGetIdentityInfo(typeof(TDomainObject));

    IdentityInfo<TDomainObject, TIdent>? TryGetIdentityInfo<TDomainObject, TIdent>()
        where TIdent : notnull => (IdentityInfo<TDomainObject, TIdent>?)this.TryGetIdentityInfo(typeof(TDomainObject));
}