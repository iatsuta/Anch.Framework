using System.Collections.Concurrent;

using Anch.IdentitySource;

namespace Anch.HierarchicalExpand;

/// <summary>Chooses the expander for a domain type: the ancestor link expander when links are registered, otherwise the pass-through expander.</summary>
/// <param name="serviceProvider">The container.</param>
/// <param name="identityInfoSource">Finds the identifier type of the domain type.</param>
/// <param name="realTypeResolver">Maps runtime types to registered domain types.</param>
public class HierarchicalObjectExpanderTypeResolver(
    IServiceProvider serviceProvider,
    IIdentityInfoSource identityInfoSource,
    IActualDomainTypeResolver realTypeResolver) : IHierarchicalObjectExpanderTypeResolver
{
    private readonly ConcurrentDictionary<Type, Type> cache = [];

    /// <inheritdoc/>
    public Type Resolve(Type domainType)
    {
        return this.cache.GetOrAdd(domainType, _ =>
        {
            var realDomainType = realTypeResolver.Resolve(domainType);

            if (realDomainType != domainType)
            {
                return this.Resolve(realDomainType);
            }
            else
            {
                var fullAncestorLinkInfo = (FullAncestorLinkInfo?)serviceProvider.GetService(typeof(FullAncestorLinkInfo<>).MakeGenericType(domainType));

                var identityInfo = identityInfoSource.GetIdentityInfo(domainType);

                if (fullAncestorLinkInfo is not null)
                {
                    return typeof(HierarchicalObjectAncestorLinkExpander<,,,>)
                        .MakeGenericType(domainType, fullAncestorLinkInfo.DirectedLinkType, fullAncestorLinkInfo.UndirectedLinkType, identityInfo.IdentityType);
                }
                else
                {
                    return typeof(PlainHierarchicalObjectExpander<>).MakeGenericType(identityInfo.IdentityType);
                }
            }
        });
    }
}