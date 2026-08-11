using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.SecuritySystem;
using Anch.SecuritySystem.Notification;
using Anch.SecuritySystem.Notification.Domain;
using Anch.SecuritySystem.Services;
using Anch.SecuritySystem.Testing;
using Anch.Testing.Xunit;

using ExampleApp.Application;
using ExampleApp.Domain;
using ExampleApp.Domain.Auth.General;

using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.IntegrationTests;

/// <summary>
/// Регрессионный тест на фикс <c>ExpandConstVisitor</c>.
///
/// Раньше (до фикса) этот запрос падал с ошибкой EF Core "could not be translated":
/// <c>InlineEvaluate</c> -> <c>ExpandConst</c> разворачивал замыкания securityContextQ/restrictionQueryable
/// (объекты IQueryable) в ConstantExpression со значением-объектом
/// (<c>value(InternalDbSet...)</c>, <c>value(EntityQueryable...)</c>), а EF Core не распознаёт
/// IQueryable-объект как корень подзапроса — нужен EntityQueryRootExpression — и строил
/// NavigationTreeExpression (p.Outer). После фикса IQueryable-константы раскрываются в своё
/// дерево, и запрос транслируется и выполняется.
///
/// Дерево строится реальными селекторами библиотеки (PermissionLevelInfoExtractor + InlineEvaluate).
/// </summary>
public class PermissionRestrictionNavigationTranslationTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    private const int AccessDenied = -2;

    private static readonly Guid TestContextId = new("11111111-1111-1111-1111-111111111111");

    private static readonly Guid BusinessUnitTypeId = new("e4ae968e-7b6b-4236-b381-9886c8e0fa34");

    [AnchFact]
    public async Task RestrictionNavigationEqualsOuterPermission_TranslatesAndExecutes(CancellationToken ct)
    {
        // Arrange
        var result = await this.GetEvaluator<IServiceProvider>()
            .EvaluateAsync(TestingScopeMode.Read, async sp =>
            {
                var queryableSource = sp.GetRequiredService<IQueryableSource>();
                var serviceProxyFactory = sp.GetRequiredService<IServiceProxyFactory>();
                var filterFactory = sp.GetRequiredService<INotificationPermissionFilterFactory<Permission>>();

                // Аналог NotificationPermissionExtractor.GetPermissionsAsync:
                // стартовый ролевой фильтр (навигация p.SecurityRole.Id == ...) + первый Select
                var startPermissionQ = queryableSource.GetQueryable<Permission>()
                    .Where(filterFactory.Create([ExampleSecurityRole.NotificationRole]))
                    .Select(p => new PermissionLevelInfo<Permission> { Permission = p, LevelInfo = "" });

                var fbu = new NotificationFilterGroup<Guid>
                {
                    SecurityContextType = typeof(BusinessUnit),
                    ExpandType = NotificationExpandType.Direct,
                    Idents = [TestContextId]
                };

                var mbu = new NotificationFilterGroup<Guid>
                {
                    SecurityContextType = typeof(ManagementUnit),
                    ExpandType = NotificationExpandType.Direct,
                    Idents = [TestContextId]
                };

                var query = ApplyNotificationFilter(startPermissionQ, serviceProxyFactory, typeof(BusinessUnit), fbu);
                query = ApplyNotificationFilter(query, serviceProxyFactory, typeof(ManagementUnit), mbu);

                // Регрессия: до фикса ExpandConstVisitor здесь бросалось InvalidOperationException
                // (NavigationTreeExpression / p.Outer / could not be translated).
                return await query.Select(pl => pl.Permission).GenericToArrayAsync(ct);
            });

        // Assert: запрос выполняется без ошибки (в пустой БД вернёт пустой результат)
        Assert.NotNull(result);
    }

    private static IQueryable<PermissionLevelInfo<Permission>> ApplyNotificationFilter(
        IQueryable<PermissionLevelInfo<Permission>> source,
        IServiceProxyFactory serviceProxyFactory,
        Type securityContextType,
        NotificationFilterGroup filterGroup)
    {
        var extractorType = typeof(PermissionLevelInfoExtractor<,>).MakeGenericType(typeof(Permission), securityContextType);

        var selector = serviceProxyFactory.Create<IPermissionLevelInfoExtractor<Permission>>(extractorType).GetSelector(filterGroup);

        return source.Select(selector)
            .Where(pl => pl.Level != AccessDenied)
            .Select(pl => new PermissionLevelInfo<Permission> { Permission = pl.Permission, LevelInfo = pl.LevelInfo });
    }

    /// <summary>
    /// Тот же запрос, что и в <see cref="RestrictionNavigationEqualsOuterPermission_TranslatesAndExecutes"/>,
    /// но с двумя вариантами restrictionsPath: библиотечным (через <c>InlineEvaluate</c>) и
    /// программным (без <c>InlineEvaluate</c>). Смысл запроса идентичен (включая сравнение
    /// навигации <c>restriction.Permission == permission</c>).
    ///
    /// До фикса <c>ExpandConstVisitor</c> библиотечный вариант падал с p.Outer, а программный
    /// выполнялся. Способ сравнения (по сущности или по Id) тут ни при чём — причина была
    /// в раскрытии IQueryable-замыканий в объект-константу. После фикса оба выполняются.
    /// </summary>
    [AnchFact]
    public async Task RestrictionNavigationEqualsOuterPermission_BothLibraryAndProgrammatic_Execute(CancellationToken ct)
    {
        await this.GetEvaluator<IServiceProvider>()
            .EvaluateAsync(TestingScopeMode.Read, async sp =>
            {
                var queryableSource = sp.GetRequiredService<IQueryableSource>();
                var filterFactory = sp.GetRequiredService<INotificationPermissionFilterFactory<Permission>>();

                var startPermissionQ = queryableSource.GetQueryable<Permission>()
                    .Where(filterFactory.Create([ExampleSecurityRole.NotificationRole]))
                    .Select(p => new PermissionLevelInfo<Permission> { Permission = p, LevelInfo = "" });

                var fbu = new NotificationFilterGroup<Guid>
                {
                    SecurityContextType = typeof(BusinessUnit),
                    ExpandType = NotificationExpandType.Direct,
                    Idents = [TestContextId]
                };

                // Библиотечный селектор (через InlineEvaluate): после фикса выполняется без p.Outer
                var libraryResult = await startPermissionQ
                    .Select(GetLibrarySelector(sp, typeof(BusinessUnit), fbu))
                    .Where(pl => pl.Level != AccessDenied)
                    .Select(pl => pl.Permission)
                    .GenericToArrayAsync(ct);

                // Программный restrictionsPath (без InlineEvaluate): тоже выполняется
                var fixedBinding = new ProgrammaticRestrictionBindingInfo(queryableSource);
                var directExtractor = sp.GetRequiredService<IDirectLevelExtractor<BusinessUnit>>();
                var fixedExtractor = new PermissionLevelInfoExtractor<Permission, BusinessUnit>(fixedBinding, directExtractor);

                var programmaticResult = await startPermissionQ
                    .Select(fixedExtractor.GetSelector(fbu))
                    .Where(pl => pl.Level != AccessDenied)
                    .Select(pl => pl.Permission)
                    .GenericToArrayAsync(ct);

                Assert.NotNull(libraryResult);
                Assert.NotNull(programmaticResult);
            });
    }

    /// <summary>
    /// Минимальный тест без выполнения EF-запроса (только построение и инспекция expression tree).
    /// Проверяет, что после фикса <c>ExpandConstVisitor</c> корень подзапроса, построенного через
    /// <c>InlineEvaluate</c> (ExpandConst), НЕ является ConstantExpression со значением-объектом
    /// IQueryable (<c>value(EntityQueryable...)</c>) — IQueryable-замыкание остаётся MemberExpression,
    /// который EF funcletize-ит в query root. Программное построение использует
    /// EntityQueryRootExpression напрямую.
    /// </summary>
    [AnchFact]
    public async Task ExpandConst_NoLonger_Embeds_Queryable_Object_Into_Constant(CancellationToken ct)
    {
        await this.GetEvaluator<IServiceProvider>()
            .EvaluateAsync(TestingScopeMode.Read, sp =>
            {
                var queryableSource = sp.GetRequiredService<IQueryableSource>();

                var businessUnitQ = queryableSource.GetQueryable<BusinessUnit>();
                var restrictionQ = queryableSource.GetQueryable<PermissionRestriction>();

                // Фактическое дерево библиотеки: InlineEvaluate -> ExpandConst. После фикса
                // IQueryable-замыкание остаётся MemberExpression (не константа с объектом).
                var inlinePath = ExpressionEvaluateHelper.InlineEvaluate<Func<Permission, IQueryable<BusinessUnit>>>(_ =>
                    permission => businessUnitQ.Where(sc =>
                        restrictionQ
                            .Where(r => r.Permission.Id == permission.Id)
                            .Select(r => r.SecurityContextId)
                            .Any(id => id == sc.Id)));

                var programmaticPath = new ProgrammaticRestrictionBindingInfo(queryableSource).GetRestrictionsPath<BusinessUnit>();

                var inlineRoot = GetSubqueryRoot(inlinePath);
                var programmaticRoot = GetSubqueryRoot(programmaticPath);

                // Раньше (до фикса) inlineRoot был ConstantExpression со значением IQueryable
                // (value(InternalDbSet...)), который EF не мог перевести.
                Assert.IsNotType<ConstantExpression>(inlineRoot);

                // Программный путь напрямую использует EntityQueryRootExpression (DbSet.Expression)
                Assert.IsNotType<ConstantExpression>(programmaticRoot);
                Assert.Contains("EntityQueryRootExpression", programmaticRoot.ToString());

                return Task.CompletedTask;
            });
    }

    /// <summary>Корень подзапроса: верхний узел — Queryable.Where(source, predicate), берём source.</summary>
    private static Expression GetSubqueryRoot<TDelegate>(Expression<TDelegate> lambda)
    {
        return ((MethodCallExpression)lambda.Body).Arguments[0];
    }

    private static Expression<Func<PermissionLevelInfo<Permission>, FullPermissionLevelInfo<Permission>>> GetLibrarySelector(
        IServiceProvider sp,
        Type securityContextType,
        NotificationFilterGroup filterGroup)
    {
        var serviceProxyFactory = sp.GetRequiredService<IServiceProxyFactory>();
        var extractorType = typeof(PermissionLevelInfoExtractor<,>).MakeGenericType(typeof(Permission), securityContextType);

        return serviceProxyFactory.Create<IPermissionLevelInfoExtractor<Permission>>(extractorType).GetSelector(filterGroup);
    }

    /// <summary>
    /// Тот же restrictionsPath, что строит GeneralPermissionTypedRestrictionBindingInfo
    /// (включая сравнение навигации restriction.Permission == permission),
    /// но построенный программно, без <c>InlineEvaluate</c>.
    /// </summary>
    private sealed class ProgrammaticRestrictionBindingInfo(IQueryableSource queryableSource) : IPermissionTypedRestrictionBindingInfo<Permission>
    {
        public Expression<Func<Permission, IQueryable<TSecurityContext>>> GetRestrictionsPath<TSecurityContext>()
            where TSecurityContext : class, ISecurityContext
        {
            var securityContextQ = queryableSource.GetQueryable<TSecurityContext>();

            var restrictionQueryable = queryableSource.GetQueryable<PermissionRestriction>();

            var permissionParam = Expression.Parameter(typeof(Permission), "permission");
            var scParam = Expression.Parameter(typeof(TSecurityContext), "sc");
            var restParam = Expression.Parameter(typeof(PermissionRestriction), "restriction");

            var scId = Expression.Property(scParam, nameof(BusinessUnit.Id));
            var restrictionPermission = Expression.Property(restParam, nameof(PermissionRestriction.Permission));
            var restrictionContextId = Expression.Property(restParam, nameof(PermissionRestriction.SecurityContextId));

            // restrictionQueryable.Where(r => r.SecurityContextType.Id == BusinessUnitTypeId)
            //            .Where(r => r.Permission == permission)
            var restrictionPermissionFilter = Expression.Equal(restrictionPermission, permissionParam);

            var typeIdFilter = Expression.Equal(
                Expression.Property(
                    Expression.Property(restParam, nameof(PermissionRestriction.SecurityContextType)), nameof(SecurityContextType.Id)),
                Expression.Constant(BusinessUnitTypeId));

            var whereRestriction = Expression.Call(
                typeof(Queryable), nameof(Queryable.Where), new[] { typeof(PermissionRestriction) },
                restrictionQueryable.Expression,
                Expression.Lambda<Func<PermissionRestriction, bool>>(
                    Expression.AndAlso(restrictionPermissionFilter, typeIdFilter), restParam));

            // .Select(r => r.SecurityContextId)
            var selectContextId = Expression.Call(
                typeof(Queryable), nameof(Queryable.Select), new[] { typeof(PermissionRestriction), typeof(Guid) },
                whereRestriction,
                Expression.Lambda<Func<PermissionRestriction, Guid>>(restrictionContextId, restParam));

            // .Any(id => id == sc.Id) — та же трёхчастная цепочка, что в оригинале
            var idParam = Expression.Parameter(typeof(Guid), "id");
            var anyRestriction = Expression.Call(
                typeof(Queryable), nameof(Queryable.Any), new[] { typeof(Guid) },
                selectContextId,
                Expression.Lambda<Func<Guid, bool>>(Expression.Equal(idParam, scId), idParam));

            // securityContextQ.Where(sc => ...)
            var whereSc = Expression.Call(
                typeof(Queryable), nameof(Queryable.Where), new[] { typeof(TSecurityContext) },
                securityContextQ.Expression,
                Expression.Lambda<Func<TSecurityContext, bool>>(anyRestriction, scParam));

            return Expression.Lambda<Func<Permission, IQueryable<TSecurityContext>>>(whereSc, permissionParam);
        }
    }
}

/// <summary>
/// Unit-тест БЕЗ БД и БЕЗ EF-сессии — только построение и инспекция expression tree.
/// Проверяет фикс <c>ExpandConstVisitor</c>: IQueryable, захваченный в замыкание, НЕ сворачивается
/// в ConstantExpression со значением-объектом («вложенная цитата») — остаётся MemberExpression.
/// EF Core сам funcletize-ит MemberExpression над IQueryable (подставляет query root),
/// а объект-константу перевести не может (value(EntityQueryable...) -> p.Outer).
/// Заодно проверяем, что обычные value-замыкания (Guid, string, ...) по-прежнему раскрываются.
/// </summary>
public class ExpandConstQueryableUnitTests
{
    [AnchFact]
    public void ExpandConst_Leaves_Queryable_Closure_As_MemberExpression(CancellationToken ct)
    {
        IQueryable<BusinessUnit> businessUnitQ = new List<BusinessUnit>().AsQueryable().Where(bu => true);
        IQueryable<PermissionRestriction> restrictionQ = new List<PermissionRestriction>().AsQueryable();
        var capturedId = Guid.NewGuid();

        Expression<Func<Permission, IQueryable<BusinessUnit>>> tree =
            permission => businessUnitQ.Where(sc =>
                restrictionQ
                    .Where(r => r.Permission == permission && r.SecurityContextType.Id == capturedId)
                    .Select(r => r.SecurityContextId)
                    .Any(id => id == sc.Id));

        var expectedRoot = GetSourceRoot(tree);
        var actualRoot = GetSourceRoot(tree.ExpandConst());

        // Источник подзапроса ДО и ПОСЛЕ ExpandConst — MemberExpression (доступ к полю замыкания),
        // а НЕ ConstantExpression со значением-объектом IQueryable (вот это EF перевести не мог).
        Assert.IsAssignableFrom<MemberExpression>(expectedRoot);
        Assert.IsAssignableFrom<MemberExpression>(actualRoot);
        Assert.IsNotType<ConstantExpression>(actualRoot);
    }

    [AnchFact]
    public void ExpandConst_Still_Expands_Value_Closures_To_Constants(CancellationToken ct)
    {
        var capturedId = Guid.NewGuid();

        Expression<Func<PermissionRestriction, bool>> tree = r => r.SecurityContextType.Id == capturedId;

        var constant = Assert.IsType<ConstantExpression>(((BinaryExpression)tree.ExpandConst().Body).Right);
        Assert.Equal(capturedId, constant.Value);
    }

    /// <summary>Корень подзапроса: верхний узел — Queryable.Where(source, predicate), берём source.</summary>
    private static Expression GetSourceRoot(Expression<Func<Permission, IQueryable<BusinessUnit>>> lambda)
    {
        return ((MethodCallExpression)lambda.Body).Arguments[0];
    }
}
