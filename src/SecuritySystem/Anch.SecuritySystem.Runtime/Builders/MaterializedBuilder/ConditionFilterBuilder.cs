using System.Collections;
using System.Linq.Expressions;

namespace Anch.SecuritySystem.Builders.MaterializedBuilder;

/// <summary>Builds the access filter with the permissions of the user loaded into memory first, for a plain condition on the object.</summary>
public class ConditionFilterBuilder<TDomainObject>(
    SecurityPath<TDomainObject>.ConditionPath securityPath)
    : SecurityFilterBuilder<TDomainObject>
{
    public override Expression<Func<TDomainObject, bool>> GetSecurityFilterExpression(IReadOnlyDictionary<Type, IEnumerable> _) =>
        securityPath.FilterExpression;
}
