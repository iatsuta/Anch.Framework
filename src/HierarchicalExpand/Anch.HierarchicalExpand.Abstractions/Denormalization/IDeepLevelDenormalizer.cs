using Anch.Core;

namespace Anch.HierarchicalExpand.Denormalization;

public interface IDeepLevelDenormalizer : IInitializer;

public interface IDeepLevelDenormalizer<in TDomainObject> : IDeepLevelDenormalizer
{
    Task UpdateDeepLevels(IEnumerable<TDomainObject> domainObjects, CancellationToken cancellationToken);
}