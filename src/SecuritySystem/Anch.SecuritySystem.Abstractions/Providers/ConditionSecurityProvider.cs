using System.Linq.Expressions;

using Anch.Core.ExpressionEvaluate;

namespace Anch.SecuritySystem.Providers;

/// <summary>A provider defined by a filter expression: objects that satisfy the filter are accessible.</summary>
/// <param name="securityFilter">The access condition.</param>
/// <param name="expressionEvaluatorStorage">Compiles the condition for checks of single objects.</param>
/// <typeparam name="TDomainObject">The type of the protected domain objects.</typeparam>
public class ConditionSecurityProvider<TDomainObject>(Expression<Func<TDomainObject, bool>> securityFilter, IExpressionEvaluatorStorage expressionEvaluatorStorage)
    : ISecurityProvider<TDomainObject>
{
    private readonly IExpressionEvaluator expressionEvaluator = expressionEvaluatorStorage.GetForType(typeof(ConditionSecurityProvider<TDomainObject>));

    /// <inheritdoc/>
    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable)
    {
        return queryable.Where(securityFilter);
    }

    /// <inheritdoc/>
    public ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken ct) =>

        new(this.expressionEvaluator.Evaluate(securityFilter, domainObject));
}