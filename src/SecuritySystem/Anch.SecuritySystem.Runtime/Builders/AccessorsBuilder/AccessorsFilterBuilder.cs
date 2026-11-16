using System.Linq.Expressions;

using Anch.HierarchicalExpand;

namespace Anch.SecuritySystem.Builders.AccessorsBuilder;

/// <summary>The base of the nodes: Computes who has access to an object (the accessors), for one node of a security path.</summary>
public abstract class AccessorsFilterBuilder<TDomainObject, TPermission>
{
    public abstract Expression<Func<TPermission, bool>> GetAccessorsFilter(TDomainObject domainObject, HierarchicalExpandType expandType);
}
