using System.Collections;
using System.Linq.Expressions;

namespace Anch.SecuritySystem.Builders.MaterializedBuilder;

/// <summary>The base of the nodes: Builds the access filter with the permissions of the user loaded into memory first, for one node of a security path.</summary>
public abstract class SecurityFilterBuilder<TDomainObject>
{
    public abstract Expression<Func<TDomainObject, bool>> GetSecurityFilterExpression(IReadOnlyDictionary<Type, IEnumerable> permission);
}
