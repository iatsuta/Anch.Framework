using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.HierarchicalExpand.Denormalization;
using Anch.HierarchicalExpand.IntegrationTests.Domain;
using Anch.HierarchicalExpand.IntegrationTests.Environment;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests;

public abstract class DenormalizeTests(IServiceProvider rootServiceProvider)
{
    private ScopeEvaluator ScopeEvaluator => field ??= rootServiceProvider.GetRequiredService<ScopeEvaluator>();

    [AnchFact]
    public async Task Initialize_Should_DenormalizeAncestors_ForLargeTree(CancellationToken ct)
    {
        await this.InitTree(ct);

        var beforeSyncState = await this.GetSyncState(ct);
        var beforeDeepLevelCorrected = await this.DeepLevelCorrected(ct);

        // Act
        await this.Denormalize(ct);

        // Assert
        var afterSyncState = await this.GetSyncState(ct);
        var afterDeepLevelCorrected = await this.DeepLevelCorrected(ct);

        Assert.Equal(7108, beforeSyncState.Adding.Count);
        Assert.Equal(SyncResult<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>.Empty, afterSyncState);

        Assert.False(beforeDeepLevelCorrected);
        Assert.True(afterDeepLevelCorrected);
    }

    [AnchFact]
    public async Task Initialize_Should_DenormalizeAncestors_AfterNodeMove_ForLargeTree(CancellationToken ct)
    {
        // Arrange
        await this.InitTree(ct);
        await this.Denormalize(ct);
        await this.MoveNode(ct);

        var beforeSyncState = await this.GetSyncState(ct);
        var beforeDeepLevelCorrected = await this.DeepLevelCorrected(ct);

        // Act
        await this.Denormalize(ct);

        // Assert
        var afterSyncState = await this.GetSyncState(ct);
        var afterDeepLevelCorrected = await this.DeepLevelCorrected(ct);

        Assert.Equal(364, beforeSyncState.Adding.Count);
        Assert.Equal(SyncResult<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>.Empty, afterSyncState);

        Assert.False(beforeDeepLevelCorrected);
        Assert.True(afterDeepLevelCorrected);
    }

    private Task InitTree(CancellationToken ct) =>
        this.ScopeEvaluator.EvaluateAsync<IGenericRepository>(async genericRepository =>
        {
            var tree = CreateTree(new TestHierarchicalObject()).ToList();

            foreach (var node in tree)
            {
                await genericRepository.SaveAsync(node, ct);
            }
        });

    private Task MoveNode(CancellationToken ct) =>
        this.ScopeEvaluator.EvaluateAsync<IServiceProvider>(async sp =>
        {
            var queryableSource = sp.GetRequiredService<IQueryableSource>();
            var genericRepository = sp.GetRequiredService<IGenericRepository>();

            var q = queryableSource.GetQueryable<TestHierarchicalObject>();

            var root = await q.Where(v => v.Parent == null).GenericSingleAsync(ct);

            var rootChildren = await Queryable.Where(q, v => v.Parent == root).GenericToListAsync(ct);

            rootChildren[0].Parent = rootChildren[1];

            await genericRepository.SaveAsync<TestHierarchicalObject>(rootChildren[0], ct);
        });

    private async Task Denormalize(CancellationToken ct)
    {
        await this.ScopeEvaluator.EvaluateAsync<IAncestorDenormalizer<TestHierarchicalObject>>(denormalizer =>
            denormalizer.Initialize(ct));

        await this.ScopeEvaluator.EvaluateAsync<IDeepLevelDenormalizer<TestHierarchicalObject>>(denormalizer =>
            denormalizer.Initialize(ct));
    }

    private Task<bool> DeepLevelCorrected(CancellationToken ct)
    {
        return this.ScopeEvaluator.EvaluateAsync(async (IQueryableSource queryableSource) =>
        {
            return await AsyncEnumerable
                .AllAsync<TestHierarchicalObject>(queryableSource
                    .GetQueryable<TestHierarchicalObject>()
                    .GenericAsAsyncEnumerable(), v => v.DeepLevel == EnumerableExtensions.GetAllElements<TestHierarchicalObject>(v, x => x.Parent, true).Count(), ct);
        });
    }

    private Task<SyncResult<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>> GetSyncState(CancellationToken ct) =>
        this.ScopeEvaluator.EvaluateAsync((IAncestorLinkExtractor<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink> ancestorLinkExtractor) =>
            ancestorLinkExtractor.GetSyncAllResult(ct));

    private static IEnumerable<TestHierarchicalObject> CreateTree(TestHierarchicalObject current, int deepSize = 6, int branchCount = 3)
    {
        yield return current;

        if (deepSize != 0)
        {
            foreach (var branchIndex in Enumerable.Range(0, branchCount))
            {
                foreach (var next in CreateTree(new TestHierarchicalObject { Parent = current }, deepSize - 1, branchCount))
                {
                    yield return next;
                }
            }
        }
    }
}