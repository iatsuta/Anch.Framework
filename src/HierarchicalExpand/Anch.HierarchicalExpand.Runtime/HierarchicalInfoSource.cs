using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand;

/// <summary>Looks up the registered hierarchy descriptions in the container.</summary>
/// <param name="serviceProvider">The container.</param>
public class HierarchicalInfoSource(IServiceProvider serviceProvider) : IHierarchicalInfoSource
{
    /// <inheritdoc/>
    public HierarchicalInfo<TDomainObject> GetHierarchicalInfo<TDomainObject>()
    {
        return serviceProvider.GetRequiredService<HierarchicalInfo<TDomainObject>>();
    }

    /// <inheritdoc/>
    public bool IsHierarchical(Type domainType)
    {
        return serviceProvider.GetService(typeof(HierarchicalInfo<>).MakeGenericType(domainType)) is not null;
    }
}