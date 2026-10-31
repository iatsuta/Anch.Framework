namespace Anch.IdentitySource;

/// <summary>Finds the identifier property of an arbitrary domain type. The property is configured explicitly or located by convention (by default a property named <c>Id</c>).</summary>
public interface IIdentityInfoSource
{
    /// <summary>Returns the identifier description of a type; throws if none can be determined.</summary>
    /// <param name="domainType">The domain type.</param>
    IdentityInfo GetIdentityInfo(Type domainType);

    /// <summary>Returns the identifier description of a type; throws if none can be determined.</summary>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    IdentityInfo<TDomainObject> GetIdentityInfo<TDomainObject>()
        => (IdentityInfo<TDomainObject>)this.GetIdentityInfo(typeof(TDomainObject));

    /// <summary>Returns the identifier description of a type with a known identifier type; throws if none can be determined.</summary>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    /// <typeparam name="TIdent">The type of the identifier.</typeparam>
    IdentityInfo<TDomainObject, TIdent> GetIdentityInfo<TDomainObject, TIdent>()
        where TIdent : notnull => (IdentityInfo<TDomainObject, TIdent>)this.GetIdentityInfo(typeof(TDomainObject));

    /// <summary>Returns the identifier description of a type, or <c>null</c> if none can be determined.</summary>
    /// <param name="domainType">The domain type.</param>
    IdentityInfo? TryGetIdentityInfo(Type domainType);

    /// <summary>Returns the identifier description of a type, or <c>null</c> if none can be determined.</summary>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    IdentityInfo<TDomainObject>? TryGetIdentityInfo<TDomainObject>()
        => (IdentityInfo<TDomainObject>?)this.TryGetIdentityInfo(typeof(TDomainObject));

    /// <summary>Returns the identifier description of a type with a known identifier type, or <c>null</c> if none can be determined.</summary>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    /// <typeparam name="TIdent">The type of the identifier.</typeparam>
    IdentityInfo<TDomainObject, TIdent>? TryGetIdentityInfo<TDomainObject, TIdent>()
        where TIdent : notnull => (IdentityInfo<TDomainObject, TIdent>?)this.TryGetIdentityInfo(typeof(TDomainObject));
}