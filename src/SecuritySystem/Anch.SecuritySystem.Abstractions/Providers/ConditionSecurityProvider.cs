using System.Linq.Expressions;

using Anch.Core.ExpressionEvaluate;

namespace Anch.SecuritySystem.Providers;

public class ConditionSecurityProvider<TDomainObject>(Expression<Func<TDomainObject, bool>> securityFilter, IExpressionEvaluatorStorage expressionEvaluatorStorage)
    : ISecurityProvider<TDomainObject>
{
    private readonly IExpressionEvaluator expressionEvaluator = expressionEvaluatorStorage.GetForType(typeof(ConditionSecurityProvider<TDomainObject>));

    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable)
    {
        return queryable.Where(securityFilter);
    }

    public ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken) =>

        new(this.expressionEvaluator.Evaluate(securityFilter, domainObject));
}