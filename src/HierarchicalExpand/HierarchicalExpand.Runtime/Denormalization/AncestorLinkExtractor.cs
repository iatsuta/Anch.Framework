using System.Collections.Immutable;

using CommonFramework;
using CommonFramework.GenericRepository;

using GenericQueryable;
using GenericQueryable.Fetching;

namespace HierarchicalExpand.Denormalization;

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

    public async Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncAllResult(CancellationToken cancellationToken)
    {
        var existsDomainObjects = await queryableSource.GetQueryable<TDomainObject>().GenericToListAsync(cancellationToken);

        var existsLinks = await queryableSource.GetQueryable<TDirectAncestorLink>().WithFetch(this.linkFetchRule).GenericToListAsync(cancellationToken);

        var nonExistsDomainObjects = existsLinks.Select(this.ToInfo).SelectMany(link => new[] { link.Ancestor, link.Child }).Except(existsDomainObjects);

        return await this.GetSyncResult(existsDomainObjects, nonExistsDomainObjects, cancellationToken);
    }

    public async Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken cancellationToken)
    {
        var domainObjectExpander = domainObjectExpanderFactory.Create();

        var existsLinkInfos = await updatedDomainObjectsBase
            .ToAsyncEnumerable()
            .Select((domainObject, ct) => this.GetSyncResult(domainObject, domainObjectExpander, ct))
            .ToArrayAsync(cancellationToken);

        var forceRemovedLinks = await this.GetExistsLinks(removedDomainObjects, cancellationToken);

        var forceRemovedLinksSyncResult = new SyncResult<TDomainObject, TDirectAncestorLink>([], forceRemovedLinks);

        return existsLinkInfos.Union([forceRemovedLinksSyncResult]).Aggregate();
    }

    public Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(TDomainObject domainObject, CancellationToken cancellationToken) =>
        this.GetSyncResult([domainObject], [], cancellationToken);

    private async ValueTask<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(
        TDomainObject domainObject,
        IDomainObjectExpander<TDomainObject> domainObjectExpander,
        CancellationToken cancellationToken)
    {
        var expectedParents = await domainObjectExpander.GetAllParents([domainObject], cancellationToken);

        var existsParents = await queryableSource
            .GetQueryable<TDirectAncestorLink>()
            .Where(this.ancestorLinkInfo.To.Path.Select(toObj => toObj == domainObject))
            .Select(this.ancestorLinkInfo.From.Path)
            .GenericToListAsync(cancellationToken);

        var mergeResult = existsParents.GetMergeResult(expectedParents);

        if (mergeResult.IsEmpty)
        {
            return SyncResult<TDomainObject, TDirectAncestorLink>.Empty;
        }
        else
        {
            var children = await domainObjectExpander.GetAllChildren([domainObject], cancellationToken);

            var addedLinks =

                from newParent in mergeResult.AddingItems

                from child in children

                select new AncestorLinkData<TDomainObject>(newParent, child);

            var removedLinks = await queryableSource
                .GetQueryable<TDirectAncestorLink>()
                .Where(this.ancestorLinkInfo.To.Path.Select(toObj => children.Contains(toObj))
                    .BuildAnd(this.ancestorLinkInfo.From.Path.Select(toObj => mergeResult.RemovingItems.Contains(toObj))))
                .GenericToListAsync(cancellationToken);

            return new(addedLinks, removedLinks);
        }
    }

    private ValueTask<ImmutableArray<TDirectAncestorLink>> GetExistsLinks(IEnumerable<TDomainObject> domainObjects, CancellationToken cancellationToken)
    {
        var filter = this.ancestorLinkInfo.From.Path.Select(fromObj => domainObjects.Contains(fromObj))
            .BuildOr(this.ancestorLinkInfo.To.Path.Select(toObj => domainObjects.Contains(toObj)));

        return queryableSource
            .GetQueryable<TDirectAncestorLink>()
            .Where(filter)
            .WithFetch(this.linkFetchRule)
            .GenericAsAsyncEnumerable()
            .ToImmutableArrayAsync(cancellationToken);
    }

    private AncestorLinkData<TDomainObject> ToInfo(TDirectAncestorLink link)
    {
        return new AncestorLinkData<TDomainObject>(this.ancestorLinkInfo.From.Getter(link), this.ancestorLinkInfo.To.Getter(link));
    }
}