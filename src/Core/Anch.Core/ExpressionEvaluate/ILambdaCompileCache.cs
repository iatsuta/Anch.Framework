using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

/// <summary>Compiles lambda expressions and caches the result.</summary>
public interface ILambdaCompileCache
{
    /// <summary>Returns the compiled delegate; lambdas that differ only in constants share one compilation.</summary>
    /// <param name="lambdaExpression">The expression.</param>
    /// <typeparam name="TDelegate">The delegate type.</typeparam>
    TDelegate GetFunc<TDelegate>(Expression<TDelegate> lambdaExpression);
}