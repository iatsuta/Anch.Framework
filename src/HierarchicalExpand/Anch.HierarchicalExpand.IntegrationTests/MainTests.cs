using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.HierarchicalExpand.Denormalization;
using Anch.HierarchicalExpand.IntegrationTests.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests;

public abstract class MainTests(IServiceProvider rootServiceProvider)
{
    [AnchFact]
    public async Task InvokeExpandWithParents_ForRootBu_DataCorrected(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        var rootBuId = await queryableSource.GetQueryable<BusinessUnit>().Where(bu => bu.Parent == null).Select(bu => bu.Id)
            .GenericSingleAsync(ct);

        // Act
        var dict = hierarchicalObjectExpander.ExpandWithParents([rootBuId], HierarchicalExpandType.Parents);

        // Assert
        Assert.Single(dict);
        Assert.Equivalent(new Dictionary<Guid, Guid> { { rootBuId, Guid.Empty } }, dict);
    }

    [AnchFact]
    public async Task InvokeChildrenExpand_ForRootBu_DataCorrected(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        var rootBuId = await queryableSource.GetQueryable<BusinessUnit>().Where(bu => bu.Parent == null).Select(bu => bu.Id)
            .GenericSingleAsync(ct);

        var expectedBuIdents = await queryableSource.GetQueryable<BusinessUnit>().Select(bu => bu.Id).OrderBy(v => v).GenericToListAsync(ct);

        // Act
        var result = hierarchicalObjectExpander.Expand([rootBuId], HierarchicalExpandType.Children).ToList();

        // Assert
        Assert.Equivalent(expectedBuIdents, result.OrderBy(v => v));
    }

    [AnchFact]
    public async Task InvokeAllExpand_ForMiddleBu_DataCorrected(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        var middleBuId = await queryableSource.GetQueryable<BusinessUnit>().Where(bu => bu.Parent != null && bu.Parent.Parent == null).Select(bu => bu.Id)
            .GenericFirstAsync(ct);

        var parentsIdents = hierarchicalObjectExpander.Expand([middleBuId], HierarchicalExpandType.Parents).ToList();
        var childrenIdents = hierarchicalObjectExpander.Expand([middleBuId], HierarchicalExpandType.Children).ToList();

        var expectedBuIdents = parentsIdents.Concat(childrenIdents).Distinct().OrderBy(v => v).ToList();

        // Act
        var result = hierarchicalObjectExpander.Expand([middleBuId], HierarchicalExpandType.All).ToList();

        // Assert
        Assert.Equivalent(expectedBuIdents, result.OrderBy(v => v));
    }

    [AnchFact]
    public async Task GetSyncAllResult_ReturnsEmpty_WhenNoChangesDetected(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var ancestorLinkExtractor = scope.ServiceProvider.GetRequiredService<IAncestorLinkExtractor<BusinessUnit, BusinessUnitDirectAncestorLink>>();

        // Act
        var result = await ancestorLinkExtractor.GetSyncAllResult(ct);

        // Assert
        Assert.Equal(SyncResult<BusinessUnit, BusinessUnitDirectAncestorLink>.Empty, result);
    }
}