using System.Linq.Expressions;

using Anch.Core.ExpressionEvaluate;
using Anch.HierarchicalExpand;

namespace Anch.SecuritySystem.Builders.QueryBuilder;

/// <summary>Builds the access filter as an expression that the database evaluates, for a plain condition on the object.</summary>
public class ConditionFilterBuilder<TDomainObject, TPermission>(
    SecurityPath<TDomainObject>.ConditionPath securityPath)
    : SecurityFilterBuilder<TDomainObject, TPermission>
{
    public override Expression<Func<TDomainObject, TPermission, bool>> GetSecurityFilterExpression(
        HierarchicalExpandType expandType)
    {
        var securityFilter = securityPath.FilterExpression;

        return ExpressionEvaluateHelper.InlineEvaluate<Func<TDomainObject, TPermission, bool>>(ee =>

            (domainObject, _) => ee.Evaluate(securityFilter, domainObject));
    }
}