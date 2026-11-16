using Anch.Core;
using Anch.GenericRepository;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.Denormalization;

/// <summary>Recomputes the stored depth of every registered type that has one; used for initialization.</summary>
/// <param name="serviceProvider">Supplies the denormalizer of each type.</param>
/// <param name="deepLevelInfoList">The registered depth descriptions.</param>
public class DeepLevelDenormalizer(IServiceProvider serviceProvider, IEnumerable<DeepLevelInfo> deepLevelInfoList)
    : IDeepLevelDenormalizer
{
    /// <inheritdoc/>
    public async Task Initialize(CancellationToken ct)
    {
        foreach (var deepLevelInfo in deepLevelInfoList)
        {
            var innerInitializer =
                (IDeepLevelDenormalizer)serviceProvider.GetRequiredService(
                    typeof(IDeepLevelDenormalizer<>).MakeGenericType(deepLevelInfo.DomainObjectType));

            await innerInitializer.Initialize(ct);
        }
    }
}

/// <summary>Keeps the stored depth of objects equal to the number of their ancestors including themselves; also updates the descendants of changed objects.</summary>
/// <param name="queryableSource">Reads the objects.</param>
/// <param name="genericRepository">Saves changed objects.</param>
/// <param name="domainObjectExpanderFactory">Creates the expander that finds descendants.</param>
/// <param name="hierarchicalInfo">The parent property.</param>
/// <param name="deepLevelInfo">The depth property.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public class DeepLevelDenormalizer<TDomainObject>(
    IQueryableSource queryableSource,
    IGenericRepository genericRepository,
    IDomainObjectExpanderFactory<TDomainObject> domainObjectExpanderFactory,
    HierarchicalInfo<TDomainObject> hierarchicalInfo,
    DeepLevelInfo<TDomainObject> deepLevelInfo) : IDeepLevelDenormalizer<TDomainObject>
    where TDomainObject : class
{
    /// <inheritdoc/>
    public async Task UpdateDeepLevels(IEnumerable<TDomainObject> domainObjects, CancellationToken ct)
    {
        var updatedDomainObjects = domainObjects.Where(domainObject =>
            deepLevelInfo.DeepLevel.Getter(domainObject)
            != domainObject.GetAllElements(hierarchicalInfo.ParentFunc, true).Count());

        foreach (var domainObject in await domainObjectExpanderFactory.Create().GetAllChildren(updatedDomainObjects, ct))
        {
            deepLevelInfo.DeepLevel.Setter.Invoke(domainObject,
                domainObject.GetAllElements(hierarchicalInfo.ParentFunc, true).Count());

            await genericRepository.SaveAsync(domainObject, ct);
        }
    }

    /// <inheritdoc/>
    public Task Initialize(CancellationToken ct) =>
        this.UpdateDeepLevels(queryableSource.GetQueryable<TDomainObject>(), ct);
}