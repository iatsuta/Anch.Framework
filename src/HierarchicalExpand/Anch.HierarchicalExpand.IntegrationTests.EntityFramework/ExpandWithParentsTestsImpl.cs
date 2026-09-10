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

        var projected = expandedDomainObjects
            .Select(ExpressionEvaluateHelper.InlineEvaluate(ee =>

                ExpressionHelper.Create((BusinessUnit domainObject) => new
                {
                    Id = ee.Evaluate(idPath, domainObject),
                    ParentId = ee.Evaluate(parentPath, domainObject) == null
                        ? default
                        : ee.Evaluate(idPath!, ee.Evaluate(parentPath, domainObject))
                })));

        var diagVisitor = new ParamDiagVisitor();
        diagVisitor.Visit(securedIdentsQueryable.Expression);
        diagVisitor.Visit(expandedDomainObjects.Expression);
        diagVisitor.Visit(parentsFilter);
        throw new Exception(diagVisitor.Report());

        var result = projected
            .Distinct()
            .ToDictionary(pair => pair.Id, pair => pair.ParentId!);

        // Assert
        Assert.Contains(middleBusinessUnit.Id, result.Keys);
        Assert.Equal(middleBusinessUnit.ParentId, result[middleBusinessUnit.Id]);
    }
}

file class ParamDiagVisitor : ExpressionVisitor
{
    private readonly Dictionary<ParameterExpression, int> declaredIn = new();

    private readonly List<string> problems = new();

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        foreach (var p in node.Parameters)
        {
            if (this.declaredIn.TryGetValue(p, out var count))
            {
                this.declaredIn[p] = count + 1;

                this.problems.Add($"ParameterExpression '{p.Name}' ({p.GetHashCode()}, type {p.Type}) declared as a Lambda parameter MORE THAN ONCE (seen {count + 1} times) — likely the same object reused across two different lambdas.");
            }
            else
            {
                this.declaredIn[p] = 1;
            }
        }

        return base.VisitLambda(node);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (!this.declaredIn.ContainsKey(node))
        {
            this.problems.Add($"ParameterExpression '{node.Name}' ({node.GetHashCode()}, type {node.Type}) referenced but NEVER declared as a Lambda parameter in the visited tree(s) — dangling/free parameter.");
        }

        return base.VisitParameter(node);
    }

    public string Report()
    {
        return this.problems.Count == 0
            ? "No parameter identity problems found."
            : string.Join(System.Environment.NewLine, this.problems);
    }
}
