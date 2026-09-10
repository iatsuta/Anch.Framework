using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.HierarchicalExpand.IntegrationTests.Domain;
using Anch.IdentitySource;
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

    [AnchFact]
    public async Task ExpandWithParents_ForQueryableMiddleBusinessUnits_ReturnsMiddleUnitsAndTheirParents(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        var expected = await queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null && businessUnit.Parent.Parent == null)
            .Select(businessUnit => new { businessUnit.Id, ParentId = businessUnit.Parent!.Id })
            .GenericToListAsync(ct);

        // idents передаются как немматериализованный IQueryable (не List/массив), как это делает
        // DefaultDomainBLLBase.GetTree в BSSFramework: this.GetSecureQueryable().Select(x => x.Id)
        var identsQueryable = queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null && businessUnit.Parent.Parent == null)
            .Select(businessUnit => businessUnit.Id);

        // Act
        var result = hierarchicalObjectExpander.ExpandWithParents(identsQueryable, HierarchicalExpandType.Parents);

        // Assert
        Assert.Equivalent(
            expected.ToDictionary(businessUnit => businessUnit.Id, businessUnit => (Guid?)businessUnit.ParentId),
            result);
    }

    [AnchFact]
    public async Task ExpandWithParents_ForQueryableBuiltFromCachedIdentityPath_ReturnsParents(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        // Тот же закешированный IIdentityInfo<BusinessUnit,Guid>, который HierarchicalObjectAncestorLinkExpander
        // сам использует внутри ExpandWithParentsImplementation (identityInfo.Id.Path) - как это происходит
        // в SingleContextFilterBuilder.GetSecurityFilterExpression (securityPath.Expression!.Select(identityInfo.Id.Path)).
        var identityInfo = scope.ServiceProvider.GetRequiredService<IIdentityInfo<BusinessUnit, Guid>>();

        var expected = await queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null && businessUnit.Parent.Parent == null)
            .Select(businessUnit => new { businessUnit.Id, ParentId = businessUnit.Parent!.Id })
            .GenericToListAsync(ct);

        // idents строится через ТОТ ЖЕ самый identityInfo.Id.Path, что и внутри expander'а -
        // это и есть отличие от предыдущего теста (там был "свежий" businessUnit => businessUnit.Id).
        var identsQueryable = queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null && businessUnit.Parent.Parent == null)
            .Select(identityInfo.Id.Path);

        // Act
        var result = hierarchicalObjectExpander.ExpandWithParents(identsQueryable, HierarchicalExpandType.Parents);

        // Assert
        Assert.Equivalent(
            expected.ToDictionary(businessUnit => businessUnit.Id, businessUnit => (Guid?)businessUnit.ParentId),
            result);
    }

    [AnchFact]
    public async Task ExpandWithParents_ForSecurityLikeFilteredQueryable_ReturnsParents(CancellationToken ct)
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

    /// <summary>
    /// Репродюсер на обычном типизированном LINQ, без Anch.Core.ExpressionEvaluate/IHierarchicalObjectExpander
    /// и без ручной сборки Expression Trees: два обычных подзапроса над BusinessUnit (аналог
    /// security-фильтра, раскрывающего разрешённые id через .Parent, и финальной проекции с
    /// null-conditional ParentId) комбинируются в один IQueryable и выполняются одним EF-запросом.
    /// </summary>
    [AnchFact]
    public async Task ExpandWithParents_TypedComposedQueryable_ReproducesEfBug(CancellationToken ct)
    {
        // Arrange
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();

        var middleBusinessUnit = await queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null && businessUnit.Parent.Parent == null)
            .Select(businessUnit => new { businessUnit.Id, ParentId = businessUnit.Parent!.Id })
            .GenericFirstAsync(ct);

        var grantedIds = new[] { middleBusinessUnit.Id };

        // "security-like" фильтр: раскрываем разрешённые id через детей (аналог GetExpandExpression(Children)),
        // не материализуя результат - обычный IQueryable<Guid>.
        var securedIdents = queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => businessUnit.Parent != null && grantedIds.Contains(businessUnit.Parent.Id))
            .Select(businessUnit => businessUnit.Id);

        // Финальный запрос комбинирует немматериализованный securedIdents (через Contains)
        // с проекцией, содержащей null-conditional ParentId - как ExpandWithParentsImplementation.
        var finalQuery = queryableSource.GetQueryable<BusinessUnit>()
            .Where(businessUnit => securedIdents.Contains(businessUnit.Id))
            .Select(businessUnit => new
            {
                businessUnit.Id,
                ParentId = businessUnit.Parent == null ? default : businessUnit.Parent.Id,
            });

        // Act
        var result = await finalQuery.GenericToListAsync(ct);

        // Assert
        Assert.Contains(result, pair => pair.Id == middleBusinessUnit.Id);
    }
}
