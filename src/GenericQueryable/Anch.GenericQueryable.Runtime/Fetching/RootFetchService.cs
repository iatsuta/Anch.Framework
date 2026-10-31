using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;

namespace Anch.GenericQueryable.Fetching;

/// <summary>Base fetch service: expands the rule to property paths, builds the chain of ORM calls for each path and caches the compiled result. ORM adapters supply the calls.</summary>
/// <param name="fetchRuleExpander">Expands rules into property paths.</param>
public abstract class RootFetchService(IFetchRuleExpander fetchRuleExpander) : IFetchService
{
    private readonly ConcurrentDictionary<Type, object> rootCache = [];

    /// <inheritdoc/>
    public virtual IQueryable<TSource> ApplyFetch<TSource>(IQueryable<TSource> source, FetchRule<TSource> fetchRule)
        where TSource : class => this.GetApplyFetchFunc(fetchRule).Invoke(source);

    private Func<IQueryable<TSource>, IQueryable<TSource>> GetApplyFetchFunc<TSource>(FetchRule<TSource> fetchRule)
        where TSource : class
    {
        return this.rootCache
            .GetOrAddAs(fetchRule.GetType(), _ => new ConcurrentDictionary<FetchRule<TSource>, Func<IQueryable<TSource>, IQueryable<TSource>>>())
            .GetOrAdd(fetchRule, _ =>
            {
                var fetchExpr = this.GetApplyFetchExpression(fetchRuleExpander.Expand(fetchRule));

                return fetchExpr.Compile();
            });
    }

    private Expression<Func<IQueryable<TSource>, IQueryable<TSource>>> GetApplyFetchExpression<TSource>(PropertyFetchRule<TSource> fetchRule)
        where TSource : class
    {
        var startState = ExpressionHelper.GetIdentity<IQueryable<TSource>>();

        return fetchRule.Paths.Aggregate(startState, (state, path) =>
        {
            var nextApplyFunc = this.GetApplyFetchExpression<TSource>(path);

            return ExpressionEvaluateHelper.InlineEvaluate<Func<IQueryable<TSource>, IQueryable<TSource>>>(ee =>

                q => ee.Evaluate(nextApplyFunc, ee.Evaluate(state, q)));
        });
    }

    private Expression<Func<IQueryable<TSource>, IQueryable<TSource>>> GetApplyFetchExpression<TSource>(LambdaExpressionPath fetchPath)
        where TSource : class
    {
        LambdaExpression startState = ExpressionHelper.GetIdentity<IQueryable<TSource>>();

        var resultBody = this
            .GetFetchMethods<TSource>(fetchPath).ZipStrong(fetchPath.Properties, (method, prop) => new { method, prop })
            .Aggregate(startState.Body, (state, pair) => Expression.Call(pair.method, state, this.GetFetchProperty(pair.prop)));

        return Expression.Lambda<Func<IQueryable<TSource>, IQueryable<TSource>>>(resultBody, startState.Parameters);
    }

    /// <summary>Adapts a property lambda before it is passed to the ORM method; the default returns it unchanged.</summary>
    /// <param name="prop">The property lambda.</param>
    protected virtual LambdaExpression GetFetchProperty(LambdaExpression prop) => prop;

    /// <summary>Returns the ORM methods (for example <c>Include</c> then <c>ThenInclude</c>) that load each step of the path.</summary>
    /// <param name="fetchPath">The path to load.</param>
    /// <typeparam name="TSource">The root entity type.</typeparam>
    protected abstract IEnumerable<MethodInfo> GetFetchMethods<TSource>(LambdaExpressionPath fetchPath)
        where TSource : class;
}