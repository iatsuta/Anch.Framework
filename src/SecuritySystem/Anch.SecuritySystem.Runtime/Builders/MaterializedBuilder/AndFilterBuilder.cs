using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem.Builders.MaterializedBuilder;

/// <summary>Builds the access filter with the permissions of the user loaded into memory first, for the conjunction of two security paths.</summary>
public class AndFilterBuilder<TDomainObject>(
    SecurityFilterBuilderFactory<TDomainObject> builderFactory,
    SecurityPath<TDomainObject>.AndSecurityPath securityPath,
    IReadOnlyList<SecurityContextRestriction> securityContextRestrictions)
    : BinaryFilterBuilder<TDomainObject, SecurityPath<TDomainObject>.AndSecurityPath>(
        builderFactory,
        securityPath,
        securityContextRestrictions)
{
    protected override Expression<Func<TArg, bool>> BuildOperation<TArg>(
        Expression<Func<TArg, bool>> arg1,
        Expression<Func<TArg, bool>> arg2) => arg1.BuildAnd(arg2);
}
