using System.Linq.Expressions;

using Anch.Core.ExpressionEvaluate;
using Anch.GenericQueryable.Fetching;

namespace Anch.GenericQueryable.Services;

/// <summary>Default executor: tries each <see cref="IMethodRedirector"/> in order and runs the first redirected call (compiled and cached).</summary>
/// <param name="methodRedirectors">Redirectors, tried in order.</param>
/// <param name="fetchService">Service that applies fetch rules.</param>
public class GenericQueryableExecutor(IEnumerable<IMethodRedirector> methodRedirectors, IFetchService fetchService) : IGenericQueryableExecutor
{
    private readonly ILambdaCompileCache compileCache = new LambdaCompileCache(LambdaCompileMode.None);

    /// <inheritdoc/>
    public IFetchService FetchService { get; } = fetchService;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">No redirector supports the call.</exception>
    public TResult Execute<TResult>(Expression<Func<TResult>> callExpression)
    {
        var redirectedExpressionRequest =

            from methodRedirector in methodRedirectors

            let result = methodRedirector.TryRedirect(callExpression)

            where result is not null

            select result;

        var redirectedExpression = redirectedExpressionRequest.FirstOrDefault()
                                   ?? throw new ArgumentOutOfRangeException(nameof(callExpression), "Expression can't be redirected");

        return this.compileCache.GetFunc(redirectedExpression).Invoke();
    }

    /// <summary>Executor for plain in-memory queryables: operations run synchronously and fetch rules are ignored.</summary>
    public static IGenericQueryableExecutor Sync { get; } =

        new GenericQueryableExecutor(
        [
            QueryableMethodRedirector.Default,
            AsyncEnumerableMethodRedirector.Default,
            new MethodRedirector(new CastToAsyncEnumerableTargetMethodExtractor())
        ], new IgnoreFetchService());
}