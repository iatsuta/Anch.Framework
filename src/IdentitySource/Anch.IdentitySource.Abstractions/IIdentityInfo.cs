using System.Linq.Expressions;

using Anch.Core;

namespace Anch.IdentitySource;

/// <summary>Describes the identifier property of a domain type and builds filters on it.</summary>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
/// <typeparam name="TIdent">The type of the identifier.</typeparam>
public interface IIdentityInfo<TDomainObject, TIdent> : IIdentityInfo<TDomainObject>
{
    /// <summary>The identifier property (path expression plus compiled getter and setter).</summary>
    PropertyAccessors<TDomainObject, TIdent> Id { get; }

    /// <summary>Builds a filter that matches objects whose identifier is one of the given values (translates to SQL <c>IN</c>).</summary>
    /// <param name="idents">The identifiers to match.</param>
    Expression<Func<TDomainObject, bool>> CreateFilter(IEnumerable<TIdent> idents);

    /// <summary>Builds a filter that matches the object with the given identifier.</summary>
    /// <param name="ident">The identifier to match.</param>
    Expression<Func<TDomainObject, bool>> CreateFilter(TIdent ident);
}

/// <summary>Describes the identifier property of a domain type.</summary>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public interface IIdentityInfo<in TDomainObject> : IIdentityInfo
{
    /// <summary>Reads the identifier of an object.</summary>
    /// <param name="domainObject">The object.</param>
    object GetId(TDomainObject domainObject);
}

/// <summary>Untyped description of the identifier of a domain type.</summary>
public interface IIdentityInfo
{
    /// <summary>The domain type.</summary>
    Type DomainObjectType { get; }

    /// <summary>The type of the identifier.</summary>
    Type IdentityType { get; }
}