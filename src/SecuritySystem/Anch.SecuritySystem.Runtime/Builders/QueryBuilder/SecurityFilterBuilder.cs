using System.Linq.Expressions;

using Anch.HierarchicalExpand;

namespace Anch.SecuritySystem.Builders.QueryBuilder;

/// <summary>The base of the nodes: Builds the access filter as an expression that the database evaluates, for one node of a security path.</summary>
public abstract class SecurityFilterBuilder<TDomainObject, TPermission>
{
    public abstract Expression<Func<TDomainObject, TPermission, bool>> GetSecurityFilterExpression(HierarchicalExpandType expandType);
}
