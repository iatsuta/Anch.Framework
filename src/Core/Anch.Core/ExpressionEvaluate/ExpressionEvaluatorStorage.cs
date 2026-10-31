using Anch.Core.DictionaryCache;

namespace Anch.Core.ExpressionEvaluate;

/// <summary>Keeps one caching evaluator per owner type.</summary>
/// <param name="lambdaCompileMode">The compile mode of the evaluators.</param>
public class ExpressionEvaluatorStorage(LambdaCompileMode lambdaCompileMode) : IExpressionEvaluatorStorage
{
    private readonly IDictionaryCache<Type, IExpressionEvaluator> cache =
        new DictionaryCache<Type, IExpressionEvaluator>(_ => new ExpressionEvaluator(new LambdaCompileCache(lambdaCompileMode))).WithLock();

    /// <inheritdoc/>
    public IExpressionEvaluator GetForType(Type type)
    {
        return this.cache[type];
    }
}