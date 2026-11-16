using Anch.Core;
using Anch.GenericRepository;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.Denormalization;

/// <summary>Rebuilds the ancestor links of every registered hierarchical type; used for initialization.</summary>
/// <param name="serviceProvider">Supplies the denormalizer of each type.</param>
/// <param name="fullAncestorLinkInfoList">The registered hierarchical types.</param>
public class AncestorDenormalizer(IServiceProvider serviceProvider, IEnumerable<FullAncestorLinkInfo> fullAncestorLinkInfoList)
    : IAncestorDenormalizer
{
    /// <inheritdoc/>
    public async Task Initialize(CancellationToken ct)
    {
        foreach (var fullAncestorLinkInfo in fullAncestorLinkInfoList)
        {
            var innerInitializer =
                (IAncestorDenormalizer)serviceProvider.GetRequiredService(
                    typeof(IAncestorDenormalizer<>).MakeGenericType(fullAncestorLinkInfo.DomainObjectType));

            await innerInitializer.Initialize(ct);
        }
    }
}

/// <summary>The denormalizer of one type; delegates to the implementation chosen through the service proxy factory.</summary>
/// <param name="serviceProxyFactory">Creates the implementation.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public class AncestorDenormalizer<TDomainObject>(IServiceProxyFactory serviceProxyFactory) : IAncestorDenormalizer<TDomainObject>
{
    private readonly IAncestorDenormalizer<TDomainObject> innerService =
        serviceProxyFactory.Create<IAncestorDenormalizer<TDomainObject>>();

    /// <inheritdoc/>
    public Task Initialize(CancellationToken ct) =>
        this.innerService.Initialize(ct);

    /// <inheritdoc/>
    public Task SyncAsync(IEnumerable<TDomainObject> updatedDomainObjectsBase, IEnumerable<TDomainObject> removedDomainObjects, CancellationToken ct) =>
        this.innerService.SyncAsync(updatedDomainObjectsBase, removedDomainObjects, ct);
}

/// <summary>Keeps the directed ancestor link entities in sync with the tree: adds the missing links and removes the obsolete ones.</summary>
/// <param name="genericRepository">Saves and removes link entities.</param>
/// <param name="fullAncestorLinkInfo">Describes the link entities.</param>
/// <param name="ancestorLinkExtractor">Computes which links to add and remove.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
/// <typeparam name="TDirectAncestorLink">The directed link entity type.</typeparam>
public class AncestorDenormalizer<TDomainObject, TDirectAncestorLink>(
    IGenericRepository genericRepository,
    FullAncestorLinkInfo<TDomainObject, TDirectAncestorLink> fullAncestorLinkInfo,
    IAncestorLinkExtractor<TDomainObject, TDirectAncestorLink> ancestorLinkExtractor) : IAncestorDenormalizer<TDomainObject>
    where TDirectAncestorLink : class, new()
    where TDomainObject : class
{
    public async Task Initialize(CancellationToken ct)
    {
        var syncResult = await ancestorLinkExtractor.GetSyncAllResult(ct);

        await this.ApplySync(syncResult, ct);
    }

    /// <inheritdoc/>
    public async Task SyncAsync(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken ct)
    {
        var syncResult = await ancestorLinkExtractor.GetSyncResult(updatedDomainObjectsBase, removedDomainObjects, ct);

        await this.ApplySync(syncResult, ct);
    }

    private async Task ApplySync(SyncResult<TDomainObject, TDirectAncestorLink> syncResult, CancellationToken ct)
    {
        foreach (var addLink in syncResult.Adding)
        {
            await this.SaveAncestor(this.CreateLink(addLink.Ancestor, addLink.Child), ct);
        }

        foreach (var removeLink in syncResult.Removing)
        {
            await this.RemoveAncestor(removeLink, ct);
        }
    }

    private async Task RemoveAncestor(TDirectAncestorLink domainObjectAncestorLink, CancellationToken ct)
    {
        await genericRepository.RemoveAsync(domainObjectAncestorLink, ct);
    }

    private async Task SaveAncestor(TDirectAncestorLink domainObjectAncestorLink, CancellationToken ct)
    {
        await genericRepository.SaveAsync(domainObjectAncestorLink, ct);
    }

    private TDirectAncestorLink CreateLink(TDomainObject ancestor, TDomainObject child)
    {
        var link = new TDirectAncestorLink();

        fullAncestorLinkInfo.Directed.From.Setter(link, ancestor);
        fullAncestorLinkInfo.Directed.To.Setter(link, child);

        return link;
    }
}