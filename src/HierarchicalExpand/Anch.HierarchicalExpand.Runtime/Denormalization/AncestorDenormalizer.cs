using Anch.Core;
using Anch.GenericRepository;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.Denormalization;

public class AncestorDenormalizer(IServiceProvider serviceProvider, IEnumerable<FullAncestorLinkInfo> fullAncestorLinkInfoList)
    : IAncestorDenormalizer
{
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

public class AncestorDenormalizer<TDomainObject>(IServiceProxyFactory serviceProxyFactory) : IAncestorDenormalizer<TDomainObject>
{
    private readonly IAncestorDenormalizer<TDomainObject> innerService =
        serviceProxyFactory.Create<IAncestorDenormalizer<TDomainObject>>();

    public Task Initialize(CancellationToken ct) =>
        this.innerService.Initialize(ct);

    public Task SyncAsync(IEnumerable<TDomainObject> updatedDomainObjectsBase, IEnumerable<TDomainObject> removedDomainObjects, CancellationToken ct) =>
        this.innerService.SyncAsync(updatedDomainObjectsBase, removedDomainObjects, ct);
}

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