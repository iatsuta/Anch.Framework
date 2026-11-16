using System.Linq.Expressions;
using System.Reflection;

using Anch.Core;

namespace Anch.GenericQueryable.Services;

/// <summary>
/// Redirects <c>GenericXxxAsync</c> calls to the <c>System.Linq.AsyncEnumerable</c> operators: the query is first converted with <c>ToAsyncEnumerable</c>,
/// and a <see cref="ValueTask{TResult}"/> result is converted to a <see cref="Task{TResult}"/>.
/// </summary>
/// <param name="targetMethodExtractor">Finds the <c>AsyncEnumerable</c> operator for a <c>GenericXxxAsync</c> method.</param>
public class AsyncEnumerableMethodRedirector(ITargetMethodExtractor targetMethodExtractor) : MethodRedirector(targetMethodExtractor)
{
    private static readonly MethodInfo ToAsyncEnumerableMethod = typeof(AsyncEnumerable).GetMethod(nameof(AsyncEnumerable.ToAsyncEnumerable))!;

    /// <inheritdoc/>
    protected override Expression CreateCallExpression(MethodInfo targetMethod, IEnumerable<Expression> args)
    {
        var cachedArgs = args.ToList();

        var firstArg = cachedArgs.First();

        var sourceType = firstArg.Type.GetInterfaceImplementationArgument(typeof(IQueryable<>))
                         ?? throw new ArgumentOutOfRangeException(nameof(args));

        var newFirstArg = Expression.Call(ToAsyncEnumerableMethod.MakeGenericMethod(sourceType), firstArg);

        var tail = cachedArgs.Skip(1);

        var newArgs = new[] { newFirstArg }.Concat(tail);

        return base.CreateCallExpression(targetMethod, newArgs).Pipe(WrapTask);
    }

    private static Expression WrapTask(Expression callExpression)
    {
        var asTaskMethod = callExpression.Type.GetMethod(nameof(ValueTask<>.AsTask));

        return asTaskMethod is null ? callExpression : Expression.Call(callExpression, asTaskMethod);
    }

    /// <summary>Redirector over <c>System.Linq.AsyncEnumerable</c> operators.</summary>
    public static IMethodRedirector Default { get; } = new AsyncEnumerableMethodRedirector(AsyncEnumerableTargetMethodExtractor.Default);
}