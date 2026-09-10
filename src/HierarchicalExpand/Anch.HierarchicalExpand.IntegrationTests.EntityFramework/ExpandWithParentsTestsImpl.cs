using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.HierarchicalExpand.IntegrationTests.Domain;
using Anch.IdentitySource;
using Anch.Testing.Xunit;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests;

public class ExpandWithParentsTestsImpl(IServiceProvider rootServiceProvider)
{
    [AnchFact]
    private async Task Problem1(CancellationToken ct)
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

    [AnchFact]
    private async Task Problem2(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();

        var middleBusinessUnit = await queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null && businessUnit.Parent.Parent == null)
            .Select(businessUnit => new { businessUnit.Id, ParentId = businessUnit.Parent!.Id })
            .GenericFirstAsync(ct);

        // Права выданы на middleBusinessUnit - как SeManager на childBu в падающем тесте BSSFramework.
        var grantedIds = new[] { middleBusinessUnit.Id };

        // Ниже - код, вытащенный напрямую из HierarchicalObjectAncestorLinkExpander.GetExpandExpression
        // (ветка HierarchicalExpandType.Children, т.е. Ancestor -> Child).
        var childrenAncestorLinkQueryable = queryableSource.GetQueryable<BusinessUnitDirectAncestorLink>();

        Expression<Func<BusinessUnitDirectAncestorLink, BusinessUnit>> ancestorPath = ancestorLink => ancestorLink.Ancestor;

        Expression<Func<BusinessUnitDirectAncestorLink, BusinessUnit>> childPath = ancestorLink => ancestorLink.Child;

        Expression<Func<BusinessUnit, Guid>> idPath = businessUnit => businessUnit.Id;

        var fromPathIdExpr = ancestorPath.Select(idPath);

        var toPathIdExpr = childPath.Select(idPath);

        var expandChildrenExpr = ExpressionEvaluateHelper.InlineEvaluate(ee =>

            ExpressionHelper.Create<IEnumerable<Guid>, IEnumerable<Guid>>(idents =>

                childrenAncestorLinkQueryable.Where(ancestorLink => idents.Contains(ee.Evaluate(fromPathIdExpr, ancestorLink)))
                    .Select(toPathIdExpr)
                    .Distinct()));

        // Это в точности то, что делает SingleContextFilterBuilder.GetSecurityFilterExpression:
        // берём expand-expression у ТОГО ЖЕ ancestor-link-запроса, которым потом будем строить дерево,
        // и фильтруем по нему queryable - т.е. security-фильтр и построение дерева
        // используют один и тот же expand-expression в одном EF-запросе.
        var securityFilter = ExpressionEvaluateHelper.InlineEvaluate(ee =>
            ExpressionHelper.Create((BusinessUnit businessUnit) =>
                ee.Evaluate(expandChildrenExpr, grantedIds).Contains(businessUnit.Id)));

        var securedIdentsQueryable = queryableSource.GetQueryable<BusinessUnit>()
            .Where(securityFilter)
            .Select(businessUnit => businessUnit.Id);

        // Act

        // Ниже - код, вытащенный напрямую из HierarchicalObjectAncestorLinkExpander.ExpandWithParents(IQueryable<TIdent>, ...)
        // -> ExpandWithParentsImplementation -> ExpandDomainObject (ветка HierarchicalExpandType.Parents, т.е. Child -> Ancestor).
        var parentsFilter = toPathIdExpr.Select(domainObjectId => securedIdentsQueryable.Contains(domainObjectId));

        var expandedDomainObjects = queryableSource.GetQueryable<BusinessUnitDirectAncestorLink>()
            .Where(parentsFilter)
            .Select(ancestorPath);

        Expression<Func<BusinessUnit, BusinessUnit?>> parentPath = businessUnit => businessUnit.Parent;

        var result = expandedDomainObjects
            .Select(ExpressionEvaluateHelper.InlineEvaluate(ee =>

                ExpressionHelper.Create((BusinessUnit domainObject) => new
                {
                    Id = ee.Evaluate(idPath, domainObject),
                    ParentId = ee.Evaluate(parentPath, domainObject) == null
                        ? default
                        : ee.Evaluate(idPath!, ee.Evaluate(parentPath, domainObject))
                })))
            .Distinct()
            .ToDictionary(pair => pair.Id, pair => pair.ParentId!);

        // Assert
        Assert.Contains(middleBusinessUnit.Id, result.Keys);
        Assert.Equal(middleBusinessUnit.ParentId, result[middleBusinessUnit.Id]);
    }
}
