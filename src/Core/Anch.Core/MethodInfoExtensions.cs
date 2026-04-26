using System.Linq.Expressions;
using System.Reflection;

namespace Anch.Core;

public static class MethodInfoExtensions
{
    extension(MethodInfo methodInfo)
    {
	    public TResult Invoke<TResult>(object? source)
	    {
		    return methodInfo.Invoke<TResult>(source, []);
	    }

	    public TResult Invoke<TResult>(object? source, IEnumerable<object?> args)
	    {
		    return (TResult)methodInfo.Invoke(source, args.ToArray())!;
	    }

	    public TResult Invoke<TResult>(object? source, object? arg1, params object?[] args)
	    {
		    return methodInfo.Invoke<TResult>(source, new[] { arg1 }.Concat(args));
	    }

	    public bool IsGenericMethodImplementation(MethodInfo genericMethod)
	    {
		    if (!genericMethod.IsGenericMethodDefinition)
		    {
			    throw new ArgumentOutOfRangeException(nameof(genericMethod));
		    }

		    return methodInfo.IsGenericMethod && methodInfo.GetGenericMethodDefinition() == genericMethod;
	    }

	    public MethodCallExpression ToCallExpression(IEnumerable<Expression> children)
	    {
		    if (methodInfo.IsStatic)
		    {
			    return Expression.Call(methodInfo, children);
		    }
		    else
		    {
			    return children.GetByFirst((first, other) => Expression.Call(first, methodInfo, other));
		    }
	    }
    }
}