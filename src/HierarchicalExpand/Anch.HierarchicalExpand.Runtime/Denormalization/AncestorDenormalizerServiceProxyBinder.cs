using Anch.Core;

namespace Anch.HierarchicalExpand.Denormalization;

public class AncestorDenormalizerServiceProxyBinder<TDomainObject>(FullAncestorLinkInfo<TDomainObject> fullAncestorLinkInfo) : IServiceProxyBinder
{
    public Type GetTargetServiceType() => typeof(AncestorDenormalizer<,>).MakeGenericType(typeof(TDomainObject), fullAncestorLinkInfo.DirectedLinkType);
}