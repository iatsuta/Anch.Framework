using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.HierarchicalExpand.IntegrationTests.Domain;
using Anch.Testing.Xunit;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests;

public class ExpandWithParentsTestsImpl(IServiceProvider rootServiceProvider)
{
    [AnchFact]
    private async Task ExpandTest2(CancellationToken ct)
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

        // Права выданы на middleBusinessUnit - как SeManager на childBu в падающем тесте BSSFramework.
        var grantedIds = new[] { middleBusinessUnit.Id };

        // Это в точности то, что делает SingleContextFilterBuilder.GetSecurityFilterExpression:
        // берём expand-expression у ТОГО ЖЕ expander'а, которым потом будем строить дерево,
        // и фильтруем по нему queryable - т.е. security-фильтр и построение дерева
        // используют один и тот же HierarchicalObjectAncestorLinkExpander в одном EF-запросе.
        var expandChildrenExpr = hierarchicalObjectExpander.GetExpandExpression(HierarchicalExpandType.Children);

        var securityFilter = ExpressionEvaluateHelper.InlineEvaluate(ee =>
            ExpressionHelper.Create((BusinessUnit businessUnit) =>
                ee.Evaluate(expandChildrenExpr, grantedIds).Contains(businessUnit.Id)));

        var securedIdentsQueryable = queryableSource.GetQueryable<BusinessUnit>()
            .Where(securityFilter)
            .Select(businessUnit => businessUnit.Id);

        // Act
        var result = hierarchicalObjectExpander.ExpandWithParents(securedIdentsQueryable, HierarchicalExpandType.Parents);

        // Assert
        Assert.Contains(middleBusinessUnit.Id, result.Keys);
        Assert.Equal(middleBusinessUnit.ParentId, result[middleBusinessUnit.Id]);
    }
}
