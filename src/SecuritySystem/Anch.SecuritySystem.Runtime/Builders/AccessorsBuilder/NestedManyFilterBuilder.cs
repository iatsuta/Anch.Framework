using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.HierarchicalExpand;

namespace Anch.SecuritySystem.Builders.AccessorsBuilder;

/// <summary>Computes who has access to an object (the accessors), for a path through nested objects (applies the nested path to each of them).</summary>
public class NestedManyFilterBuilder<TDomainObject, TPermission, TNestedObject>(
    IExpressionEvaluatorStorage expressionEvaluatorStorage,
    AccessorsFilterBuilderFactory<TNestedObject, TPermission> nestedBuilderFactory,
    SecurityPath<TDomainObject>.NestedManySecurityPath<TNestedObject> securityPath,
    IReadOnlyList<SecurityContextRestriction> securityContextRestrictions) : AccessorsFilterBuilder<TDomainObject, TPermission>
{
    private readonly IExpressionEvaluator expressionEvaluator =
        expressionEvaluatorStorage.GetForType(typeof(NestedManyFilterBuilder<TDomainObject, TPermission, TNestedObject>));

    private AccessorsFilterBuilder<TNestedObject, TPermission> NestedBuilder { get; } =
        nestedBuilderFactory.CreateBuilder(securityPath.NestedSecurityPath, securityContextRestrictions);

    public override Expression<Func<TPermission, bool>> GetAccessorsFilter(TDomainObject domainObject, HierarchicalExpandType expandType) =>

        this.expressionEvaluator.Evaluate(securityPath.NestedExpression, domainObject).BuildOr(item => this.NestedBuilder.GetAccessorsFilter(item, expandType));
}