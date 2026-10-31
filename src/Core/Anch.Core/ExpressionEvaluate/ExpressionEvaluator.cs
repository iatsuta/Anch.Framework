using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

/// <summary>An evaluator that compiles through a compile cache.</summary>
/// <param name="lambdaCompileCache">The cache that compiles the expressions.</param>
public class ExpressionEvaluator(ILambdaCompileCache lambdaCompileCache) : IExpressionEvaluator
{
    public TDelegate Compile<TDelegate>(Expression<TDelegate> expression) => lambdaCompileCache.GetFunc(expression);
}