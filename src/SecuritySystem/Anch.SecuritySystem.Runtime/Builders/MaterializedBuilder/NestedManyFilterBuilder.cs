using System.Collections;
using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;

namespace Anch.SecuritySystem.Builders.MaterializedBuilder;

/// <summary>Builds the access filter with the permissions of the user loaded into memory first, for a path through nested objects (applies the nested path to each of them).</summary>
public class NestedManyFilterBuilder<TDomainObject, TNestedObject>(
    SecurityFilterBuilderFactory<TNestedObject> nestedBuilderFactory,
    SecurityPath<TDomainObject>.NestedManySecurityPath<TNestedObject> securityPath,
    IReadOnlyList<SecurityContextRestriction> securityContextRestrictions) : SecurityFilterBuilder<TDomainObject>
{
    private SecurityFilterBuilder<TNestedObject> NestedBuilder { get; } = nestedBuilderFactory.CreateBuilder(securityPath.NestedSecurityPath, securityContextRestrictions);

    public override Expression<Func<TDomainObject, bool>> GetSecurityFilterExpression(IReadOnlyDictionary<Type, IEnumerable> permission)
    {
        var nestedFilterExpression = this.NestedBuilder.GetSecurityFilterExpression(permission);

        var nestedCollectionFilterExpression = nestedFilterExpression.ToCollectionFilter();

        var mainCondition = ExpressionEvaluateHelper.InlineEvaluate(ee => securityPath.NestedExpression.Select(v => ee.Evaluate(nestedCollectionFilterExpression, v).Any()));

        if (securityPath.Required)
        {
            return mainCondition;
        }
        else
        {
            var emptyCondition = securityPath.NestedExpression.Select(v => !v.Any());

            return emptyCondition.BuildOr(mainCondition);
        }
    }
}