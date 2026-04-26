using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using Anch.Core.ExpressionComparers;

namespace Anch.Core.ExpressionEvaluate;

public class LambdaCompileCache(LambdaCompileMode mode, IEqualityComparer<LambdaExpression> lambdaComparer) : ILambdaCompileCache
{
	public LambdaCompileCache(LambdaCompileMode mode)
		: this(mode, ExpressionComparer.WithoutConst.LambdaComparer)
	{
	}

	private readonly ConcurrentDictionary<ValueTuple<Type, MethodInfo?>, ConcurrentDictionary<LambdaExpression, Delegate>> rootCache = [];

	public TDelegate GetFunc<TDelegate>(Expression<TDelegate> lambdaExpression)
	{
		IReadOnlyCollection<ValueTuple<ParameterExpression, ConstantExpression>> args = null!;

		var newLambdaExpression =

			lambdaExpression.Pipe(mode.HasFlag(LambdaCompileMode.OptimizeBooleanLogic), lambda => lambda.Optimize())
				.Pipe(lambda => ConstantToParameters(lambda, out args));

		var getDelegateFunc = this.GetGetDelegate<TDelegate>(newLambdaExpression, args!.Select(v => v.Item1));

		return (TDelegate)getDelegateFunc.DynamicInvoke(args.Select(v => v.Item2.Value).ToArray())!;
	}

	private Delegate GetGetDelegate<TDelegate>(LambdaExpression expr, IEnumerable<ParameterExpression> parameters)
	{
		return
			this.rootCache
				.GetOrAdd(ValueTuple.Create(typeof(TDelegate), (expr.Body as MethodCallExpression)?.Method),
					_ => new ConcurrentDictionary<LambdaExpression, Delegate>(lambdaComparer))
				.GetOrAdd(expr, _ =>
					expr.Pipe(mode.HasFlag(LambdaCompileMode.IgnoreStringCase),
							lambda => lambda.UpdateBodyBase(new OverrideStringEqualityExpressionVisitor(StringComparison.CurrentCultureIgnoreCase)))
						.Pipe(mode.HasFlag(LambdaCompileMode.InjectMaybe), InjectMaybeVisitor.Value.VisitAndGetValueOrDefaultBase)
						.Pipe(body => Expression.Lambda(body, parameters))
						.Compile());
	}

	private static LambdaExpression ConstantToParameters(LambdaExpression lambdaExpression,
		out IReadOnlyCollection<ValueTuple<ParameterExpression, ConstantExpression>> args)
	{
		var listArgs = new List<ValueTuple<ParameterExpression, ConstantExpression>>();

		var newExpression = lambdaExpression.UpdateBase(new ConstantToParameterExpressionVisitor(listArgs));

		args = listArgs;

		return (LambdaExpression)newExpression;
	}

	private class ConstantToParameterExpressionVisitor(List<ValueTuple<ParameterExpression, ConstantExpression>> args) : ExpressionVisitor
	{
		protected override Expression VisitConstant(ConstantExpression node)
		{
			var newParameter = Expression.Parameter(node.Type, "OverrideConst_" + args.Count);

			args.Add(ValueTuple.Create(newParameter, node));

			return newParameter;
		}
	}
}