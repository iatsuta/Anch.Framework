using System.Linq.Expressions;
using Anch.HierarchicalExpand;

namespace Anch.SecuritySystem.Builders.AccessorsBuilder;

public abstract class AccessorsFilterBuilder<TDomainObject, TPermission>
{
    public abstract Expression<Func<TPermission, bool>> GetAccessorsFilter(TDomainObject domainObject, HierarchicalExpandType expandType);
}
