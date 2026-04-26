using Anch.Core;

namespace Anch.HierarchicalExpand;

public class DomainObjectExpanderFactory<TDomainObject>(IServiceProxyFactory serviceProxyFactory) : IDomainObjectExpanderFactory<TDomainObject>
    where TDomainObject : class
{
    public IDomainObjectExpander<TDomainObject> Create()
    {
        return serviceProxyFactory.Create<IDomainObjectExpander<TDomainObject>, DomainObjectExpander<TDomainObject>>();
    }
}