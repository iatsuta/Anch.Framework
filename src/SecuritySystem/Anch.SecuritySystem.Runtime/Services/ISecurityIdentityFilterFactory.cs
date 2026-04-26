using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

public interface ISecurityIdentityFilterFactory<TDomainObject>
{
	Expression<Func<TDomainObject, bool>> CreateFilter(SecurityIdentity securityIdentity);
}