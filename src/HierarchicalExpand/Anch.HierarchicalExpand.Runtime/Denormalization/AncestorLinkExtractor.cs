using System.Collections.Immutable;

using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericQueryable.Fetching;
using Anch.GenericRepository;

namespace Anch.HierarchicalExpand.Denormalization;

public class AncestorLinkExtractor<TDomainObject, TDirectAncestorLink>(
    IQueryableSource queryableSource,
    IDomainObjectExpanderFactory<TDomainObject> domainObjectExpanderFactory,
    FullAncestorLinkInfo<TDomainObject, TDirectAncestorLink> fullAncestorLinkInfo)
    : IAncestorLinkExtractor<TDomainObject, TDirectAncestorLink>
    where TDirectAncestorLink : class
    where TDomainObject : class
{
    private readonly AncestorLinkInfo<TDomainObject, TDirectAncestorLink> ancestorLinkInfo = fullAncestorLinkInfo.Directed;

    private readonly FetchRule<TDirectAncestorLink> linkFetchRule =
        FetchRule<TDirectAncestorLink>.Create(fullAncestorLinkInfo.Directed.From.Path).Fetch(fullAncestorLinkInfo.Directed.To.Path);

    public async Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncAllResult(CancellationToken ct)
    {
        var existsDomainObjects = await queryableSource.GetQueryable<TDomainObject>().GenericToListAsync(ct);

        var existsLinks = await queryableSource.GetQueryable<TDirectAncestorLink>().WithFetch(this.linkFetchRule).GenericToListAsync(ct);

        var nonExistsDomainObjects = existsLinks.Select(this.ToInfo).SelectMany(link => new[] { link.Ancestor, link.Child }).Except(existsDomainObjects);

        return await this.GetSyncResult(existsDomainObjects, nonExistsDomainObjects, ct);
    }

    public async Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken ct)
    {
        var domainObjectExpander = domainObjectExpanderFactory.Create();

        var existsLinkInfos = await updatedDomainObjectsBase
            .ToAsyncEnumerable()
            .Select(async (domainObject, lct) => await this.GetSyncResult(domainObject, domainObjectExpander, lct))
            .ToArrayAsync(ct);

        var forceRemovedLinks = await this.GetExistsLinks(removedDomainObjects, ct);

        var forceRemovedLinksSyncResult = new SyncResult<TDomainObject, TDirectAncestorLink>([], forceRemovedLinks);

        return existsLinkInfos.Union([forceRemovedLinksSyncResult]).Aggregate();
    }

    public Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(TDomainObject domainObject, CancellationToken ct) =>
        this.GetSyncResult([domainObject], [], ct);

    private async Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(
        TDomainObject domainObject,
        IDomainObjectExpander<TDomainObject> domainObjectExpander,
        CancellationToken ct)
    {
        var expectedParents = await domainObjectExpander.GetAllParents([domainObject], ct);

        var existsParents = await queryableSource
            .GetQueryable<TDirectAncestorLink>()
            .Where(this.ancestorLinkInfo.To.Path.Select(toObj => toObj == domainObject))
            .Select(this.ancestorLinkInfo.From.Path)
            .GenericToListAsync(ct);

        var mergeResult = existsParents.GetMergeResult(expectedParents);

        if (mergeResult.IsEmpty)
        {
            return SyncResult<TDomainObject, TDirectAncestorLink>.Empty;
        }
        else
        {
            var children = await domainObjectExpander.GetAllChildren([domainObject], ct);

            var addedLinks =

                from newParent in mergeResult.AddingItems

                from child in children

                select new AncestorLinkData<TDomainObject>(newParent, child);

            var removedLinks = await queryableSource
                .GetQueryable<TDirectAncestorLink>()
                .Where(this.ancestorLinkInfo.To.Path.Select(toObj => children.Contains(toObj))
                    .BuildAnd(this.ancestorLinkInfo.From.Path.Select(toObj => mergeResult.RemovingItems.Contains(toObj))))
                .GenericToListAsync(ct);

            return new(addedLinks, removedLinks);
        }
    }

    private async Task<ImmutableArray<TDirectAncestorLink>> GetExistsLinks(IEnumerable<TDomainObject> domainObjects, CancellationToken ct)
    {
        var filter = this.ancestorLinkInfo.From.Path.Select(fromObj => domainObjects.Contains(fromObj))
            .BuildOr(this.ancestorLinkInfo.To.Path.Select(toObj => domainObjects.Contains(toObj)));

        return await queryableSource
            .GetQueryable<TDirectAncestorLink>()
            .Where(filter)
            .WithFetch(this.linkFetchRule)
            .GenericAsAsyncEnumerable()
            .ToImmutableArrayAsync(ct);
    }

    private AncestorLinkData<TDomainObject> ToInfo(TDirectAncestorLink link)
    {
        return new AncestorLinkData<TDomainObject>(this.ancestorLinkInfo.From.Getter(link), this.ancestorLinkInfo.To.Getter(link));
    }
}