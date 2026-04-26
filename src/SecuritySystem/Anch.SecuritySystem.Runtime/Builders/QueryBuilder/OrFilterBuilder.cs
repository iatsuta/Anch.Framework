using System.Linq.Expressions;
using Anch.Core;

namespace Anch.SecuritySystem.Builders.QueryBuilder;

public class OrFilterBuilder<TDomainObject, TPermission>(
    SecurityFilterBuilderFactory<TDomainObject, TPermission> builderFactory,
    SecurityPath<TDomainObject>.OrSecurityPath securityPath,
    IReadOnlyList<SecurityContextRestriction> securityContextRestrictions)
    : BinaryFilterBuilder<TDomainObject, TPermission, SecurityPath<TDomainObject>.OrSecurityPath>(builderFactory, securityPath, securityContextRestrictions)
{
    protected override Expression<Func<TArg1, TArg2, bool>> BuildOperation<TArg1, TArg2>(
        Expression<Func<TArg1, TArg2, bool>> arg1,
        Expression<Func<TArg1, TArg2, bool>> arg2) => arg1.BuildOr(arg2);
}
