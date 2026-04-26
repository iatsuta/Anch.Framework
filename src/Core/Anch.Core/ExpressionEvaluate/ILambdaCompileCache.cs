using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

public interface ILambdaCompileCache
{
    TDelegate GetFunc<TDelegate>(Expression<TDelegate> lambdaExpression);
}