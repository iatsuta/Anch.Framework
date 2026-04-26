using System.Linq.Expressions;

using Anch.Core;
using Anch.IdentitySource;

namespace Anch.SecuritySystem.Builders.MaterializedBuilder;

public class ManyContextFilterBuilder<TDomainObject, TSecurityContext, TSecurityContextIdent>(
	SecurityPath<TDomainObject>.ManySecurityPath<TSecurityContext> securityPath,
	SecurityContextRestriction<TSecurityContext>? securityContextRestriction,
	IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
	: ByIdentsFilterBuilder<TDomainObject, TSecurityContext, TSecurityContextIdent>(securityContextRestriction)
	where TSecurityContext : class, ISecurityContext
	where TSecurityContextIdent : notnull
{
	protected override Expression<Func<TDomainObject, bool>> GetSecurityFilterExpression(IEnumerable<TSecurityContextIdent> permissionIdents)
	{
		var singleFilter = identityInfo.CreateFilter(permissionIdents);

		var containsFilterExpr = securityPath.Expression.Select(singleFilter.ToCollectionFilter()).Select(securityContext => securityContext.Any());

		if (securityPath.Required)
		{
            return containsFilterExpr;
        }
		else
		{
            var unrestrictedFilter = securityPath.Expression.Select(securityObjects => !securityObjects.Any());

            return unrestrictedFilter.BuildOr(containsFilterExpr);
        }
	}
}