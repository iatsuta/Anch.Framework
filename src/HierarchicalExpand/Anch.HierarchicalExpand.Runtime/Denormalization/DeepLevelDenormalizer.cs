using Anch.Core;
using Anch.GenericRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.Denormalization;

public class DeepLevelDenormalizer(IServiceProvider serviceProvider, IEnumerable<DeepLevelInfo> deepLevelInfoList)
    : IDeepLevelDenormalizer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        foreach (var deepLevelInfo in deepLevelInfoList)
        {
            var innerInitializer =
                (IDeepLevelDenormalizer)serviceProvider.GetRequiredService(
                    typeof(IDeepLevelDenormalizer<>).MakeGenericType(deepLevelInfo.DomainObjectType));

            await innerInitializer.Initialize(cancellationToken);
        }
    }
}

public class DeepLevelDenormalizer<TDomainObject>(
    IQueryableSource queryableSource,
    IGenericRepository genericRepository,
    IDomainObjectExpanderFactory<TDomainObject> domainObjectExpanderFactory,
    HierarchicalInfo<TDomainObject> hierarchicalInfo,
    DeepLevelInfo<TDomainObject> deepLevelInfo) : IDeepLevelDenormalizer<TDomainObject>
    where TDomainObject : class
{
    public async Task UpdateDeepLevels(IEnumerable<TDomainObject> domainObjects, CancellationToken cancellationToken)
    {
        var updatedDomainObjects = domainObjects.Where(domainObject =>
            deepLevelInfo.DeepLevel.Getter(domainObject)
            != domainObject.GetAllElements(hierarchicalInfo.ParentFunc, true).Count());

        foreach (var domainObject in await domainObjectExpanderFactory.Create().GetAllChildren(updatedDomainObjects, cancellationToken))
        {
            deepLevelInfo.DeepLevel.Setter.Invoke(domainObject,
                domainObject.GetAllElements(hierarchicalInfo.ParentFunc, true).Count());

            await genericRepository.SaveAsync(domainObject, cancellationToken);
        }
    }

    public Task Initialize(CancellationToken cancellationToken) =>
        this.UpdateDeepLevels(queryableSource.GetQueryable<TDomainObject>(), cancellationToken);
}