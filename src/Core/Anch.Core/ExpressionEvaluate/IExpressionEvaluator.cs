using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

/// <summary>Compiles and runs expression trees, usually with a cache so the compilation cost is paid once.</summary>
public interface IExpressionEvaluator
{
    /// <summary>Compiles the expression and runs it with an argument.</summary>
    /// <param name="expression">The expression.</param>
    /// <param name="arg">The argument.</param>
    TResult Evaluate<TSource, TResult>(Expression<Func<TSource, TResult>> expression, TSource arg) => this.Compile(expression).Invoke(arg);

    /// <summary>Compiles the expression and runs it with two arguments.</summary>
    /// <param name="expression">The expression.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    TResult Evaluate<TArg1, TArg2, TResult>(Expression<Func<TArg1, TArg2, TResult>> expression, TArg1 arg1, TArg2 arg2) => this.Compile(expression).Invoke(arg1, arg2);

    /// <summary>Compiles the expression into a delegate.</summary>
    /// <param name="expression">The expression.</param>
    /// <typeparam name="TDelegate">The delegate type.</typeparam>
    TDelegate Compile<TDelegate>(Expression<TDelegate> expression);
}