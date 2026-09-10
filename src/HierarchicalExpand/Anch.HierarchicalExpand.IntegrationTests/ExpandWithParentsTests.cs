using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.HierarchicalExpand.IntegrationTests.Domain;
using Anch.Testing.Xunit;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests;

public abstract class ExpandWithParentsTests(IServiceProvider rootServiceProvider)
{
    [AnchFact]
    public async Task ExpandWithParents_ForSelectedBusinessUnits_ReturnsTheirParents(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        var businessUnitIdents = queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null)
            .Select(businessUnit => businessUnit.Id);

        var expected = (await queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null)
            .Select(businessUnit => new { businessUnit.Id, ParentId = businessUnit.Parent!.Id })
            .GenericToListAsync(ct))
            .ToDictionary(businessUnit => businessUnit.Id, businessUnit => businessUnit.ParentId);

        // Act
        var result = hierarchicalObjectExpander.ExpandWithParents(businessUnitIdents, HierarchicalExpandType.None);

        // Assert
        Assert.Equivalent(expected, result);
    }

    [AnchFact]
    public async Task ExpandWithParents_ForMiddleBusinessUnit_ReturnsMiddleUnitAndItsParent(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        var middleBusinessUnit = await queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null && businessUnit.Parent.Parent == null)
            .Select(businessUnit => new { businessUnit.Id, ParentId = businessUnit.Parent!.Id })
            .GenericFirstAsync(ct);

        // Act
        var result = hierarchicalObjectExpander.ExpandWithParents([middleBusinessUnit.Id], HierarchicalExpandType.Parents);

        // Assert
        Assert.Equivalent(
            new Dictionary<Guid, Guid?>
            {
                [middleBusinessUnit.Id] = middleBusinessUnit.ParentId,
            },
            result);
    }
}
