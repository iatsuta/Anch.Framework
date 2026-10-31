using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

/// <summary>Builds filters on the identifier of a domain type from security identifiers.</summary>
public interface ISecurityIdentityFilterFactory<TDomainObject>
{
    Expression<Func<TDomainObject, bool>> CreateFilter(SecurityIdentity securityIdentity);
}