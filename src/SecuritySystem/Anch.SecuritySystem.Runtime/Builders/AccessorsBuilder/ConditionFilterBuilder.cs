using System.Linq.Expressions;
using Anch.Core.ExpressionEvaluate;
using Anch.HierarchicalExpand;

namespace Anch.SecuritySystem.Builders.AccessorsBuilder;

public class ConditionFilterBuilder<TDomainObject, TPermission>(
    IExpressionEvaluatorStorage expressionEvaluatorStorage,
    SecurityPath<TDomainObject>.ConditionPath securityPath)
    : AccessorsFilterBuilder<TDomainObject, TPermission>
{
    private readonly IExpressionEvaluator expressionEvaluator = expressionEvaluatorStorage.GetForType(typeof(ConditionFilterBuilder<TDomainObject, TPermission>));

    public override Expression<Func<TPermission, bool>> GetAccessorsFilter(TDomainObject domainObject, HierarchicalExpandType expandType)
    {
        var hasAccess = this.expressionEvaluator.Evaluate(securityPath.FilterExpression, domainObject);

        return _ => hasAccess;
    }
}