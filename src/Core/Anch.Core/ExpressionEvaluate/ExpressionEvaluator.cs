using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

public class ExpressionEvaluator(ILambdaCompileCache lambdaCompileCache) : IExpressionEvaluator
{
    public TDelegate Compile<TDelegate>(Expression<TDelegate> expression) => lambdaCompileCache.GetFunc(expression);
}