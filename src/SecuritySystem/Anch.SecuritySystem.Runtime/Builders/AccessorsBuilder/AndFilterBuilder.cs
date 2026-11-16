using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem.Builders.AccessorsBuilder;

/// <summary>Computes who has access to an object (the accessors), for the conjunction of two security paths.</summary>
public class AndFilterBuilder<TDomainObject, TPermission>(
    AccessorsFilterBuilderFactory<TDomainObject, TPermission> builderFactory,
    SecurityPath<TDomainObject>.AndSecurityPath securityPath,
    IReadOnlyList<SecurityContextRestriction> securityContextRestrictions)
    : BinaryFilterBuilder<TDomainObject, TPermission, SecurityPath<TDomainObject>.AndSecurityPath>(
        builderFactory,
        securityPath,
        securityContextRestrictions)
{
    protected override Expression<Func<TArg, bool>> BuildOperation<TArg>(
        Expression<Func<TArg, bool>> arg1,
        Expression<Func<TArg, bool>> arg2) => arg1.BuildAnd(arg2);
}
