using System.Collections;
using System.Collections.Frozen;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.HierarchicalExpand;
using Anch.IdentitySource;
using Anch.SecuritySystem.Builders._Factory;
using Anch.SecuritySystem.Builders._Filter;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.PermissionOptimization;

namespace Anch.SecuritySystem.Builders.MaterializedBuilder;

public class SecurityFilterBuilderFactory<TDomainObject>(
    IIdentityInfoSource identityInfoSource,
	IExpressionEvaluatorStorage expressionEvaluatorStorage,
    IEnumerable<IPermissionSystem> permissionSystems,
    IHierarchicalObjectExpanderFactory hierarchicalObjectExpanderFactory,
    IRuntimePermissionOptimizationService permissionOptimizationService) :
    FilterBuilderFactoryBase<TDomainObject, SecurityFilterBuilder<TDomainObject>>(identityInfoSource),
    ISecurityFilterFactory<TDomainObject>
{
    private readonly IExpressionEvaluator expressionEvaluator =
        expressionEvaluatorStorage.GetForType(typeof(SecurityFilterBuilderFactory<TDomainObject>));

    public async ValueTask<SecurityFilterInfo<TDomainObject>> CreateFilterAsync(
        DomainSecurityRule.RoleBaseSecurityRule securityRule,
        SecurityPath<TDomainObject> securityPath,
        CancellationToken cancellationToken)
    {
        var securityContextRestrictions = securityRule.GetSafeSecurityContextRestrictions().ToList();

        var rawPermissions = await permissionSystems
            .ToAsyncEnumerable()
            .SelectMany(ps => ps.GetPermissionSources(securityRule))
            .SelectMany(ps => ps.GetPermissionsAsync(securityPath.UsedSecurityContextTypes))
            .ToListAsync(cancellationToken);

        var optimizedPermissions = permissionOptimizationService.Optimize(rawPermissions);

        var expandedPermissions =
            optimizedPermissions.Select(permission => this.TryExpandPermission(permission, securityRule.GetSafeExpandType()));

        var builder = this.CreateBuilder(securityPath, securityContextRestrictions);

        var filterExpression = expandedPermissions.BuildOr(builder.GetSecurityFilterExpression).ExpandConst();

        var lazyHasAccessFunc = LazyHelper.Create(
            () => filterExpression.UpdateBody(CacheContainsCallVisitor.Value).Pipe(this.expressionEvaluator.Compile));

        return new SecurityFilterInfo<TDomainObject>(
            q => q.Where(filterExpression),
            v => lazyHasAccessFunc.Value(v));
    }

    protected override SecurityFilterBuilder<TDomainObject> CreateBuilder(SecurityPath<TDomainObject>.ConditionPath securityPath)
    {
        return new ConditionFilterBuilder<TDomainObject>(securityPath);
    }

    protected override SecurityFilterBuilder<TDomainObject> CreateBuilder<TSecurityContext, TSecurityContextIdent>(
        SecurityPath<TDomainObject>.SingleSecurityPath<TSecurityContext> securityPath,
        SecurityContextRestriction<TSecurityContext>? securityContextRestriction,
        IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
    {
        return new SingleContextFilterBuilder<TDomainObject, TSecurityContext, TSecurityContextIdent>(securityPath, securityContextRestriction, identityInfo);
    }

    protected override SecurityFilterBuilder<TDomainObject> CreateBuilder<TSecurityContext, TSecurityContextIdent>(
        SecurityPath<TDomainObject>.ManySecurityPath<TSecurityContext> securityPath,
        SecurityContextRestriction<TSecurityContext>? securityContextRestriction,
        IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
    {
        return new ManyContextFilterBuilder<TDomainObject, TSecurityContext, TSecurityContextIdent>(securityPath, securityContextRestriction, identityInfo);
    }

    protected override SecurityFilterBuilder<TDomainObject> CreateBuilder(SecurityPath<TDomainObject>.OrSecurityPath securityPath, IReadOnlyList<SecurityContextRestriction> securityContextRestrictions)
    {
        return new OrFilterBuilder<TDomainObject>(this, securityPath, securityContextRestrictions);
    }

    protected override SecurityFilterBuilder<TDomainObject> CreateBuilder(SecurityPath<TDomainObject>.AndSecurityPath securityPath, IReadOnlyList<SecurityContextRestriction> securityContextRestrictions)
    {
        return new AndFilterBuilder<TDomainObject>(this, securityPath, securityContextRestrictions);
    }

    protected override SecurityFilterBuilder<TDomainObject> CreateBuilder<TNestedObject>(
        SecurityPath<TDomainObject>.NestedManySecurityPath<TNestedObject> securityPath,
        IReadOnlyList<SecurityContextRestriction> securityContextRestrictions)
    {
        var nestedBuilderFactory = new SecurityFilterBuilderFactory<TNestedObject>(
            identityInfoSource,
            expressionEvaluatorStorage,
            permissionSystems,
            hierarchicalObjectExpanderFactory,
            permissionOptimizationService);

        return new NestedManyFilterBuilder<TDomainObject, TNestedObject>(nestedBuilderFactory, securityPath, securityContextRestrictions);
    }

    private FrozenDictionary<Type, IEnumerable> TryExpandPermission(
        IReadOnlyDictionary<Type, Array> permission,
        HierarchicalExpandType expandType)
    {
        return permission.ToFrozenDictionary(
            pair => pair.Key,
            pair => hierarchicalObjectExpanderFactory.Create(pair.Key).Expand(pair.Value, expandType));
    }
}
