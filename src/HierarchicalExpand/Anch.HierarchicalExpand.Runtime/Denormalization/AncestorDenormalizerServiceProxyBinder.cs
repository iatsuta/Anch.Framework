using Anch.Core;

namespace Anch.HierarchicalExpand.Denormalization;

/// <summary>Chooses the ancestor denormalizer implementation for a domain type from the type of its directed link.</summary>
/// <param name="fullAncestorLinkInfo">Describes the link entities of the type.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public class AncestorDenormalizerServiceProxyBinder<TDomainObject>(FullAncestorLinkInfo<TDomainObject> fullAncestorLinkInfo) : IServiceProxyBinder
{
    /// <inheritdoc/>
    public Type GetTargetServiceType() => typeof(AncestorDenormalizer<,>).MakeGenericType(typeof(TDomainObject), fullAncestorLinkInfo.DirectedLinkType);
}