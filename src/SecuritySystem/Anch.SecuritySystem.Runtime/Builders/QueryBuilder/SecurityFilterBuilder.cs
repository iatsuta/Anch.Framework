using System.Linq.Expressions;
using Anch.HierarchicalExpand;

namespace Anch.SecuritySystem.Builders.QueryBuilder;

public abstract class SecurityFilterBuilder<TDomainObject, TPermission>
{
    public abstract Expression<Func<TDomainObject, TPermission, bool>> GetSecurityFilterExpression(HierarchicalExpandType expandType);
}
