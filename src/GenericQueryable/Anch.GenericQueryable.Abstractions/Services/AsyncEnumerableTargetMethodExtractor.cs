using System.Collections.Immutable;
using System.Reflection;

using Anch.Core;

namespace Anch.GenericQueryable.Services;

/// <summary>Maps <c>GenericXxxAsync</c> to the operator of the same name on <c>System.Linq.AsyncEnumerable</c>, whose first parameter is an <see cref="IAsyncEnumerable{T}"/>.</summary>
/// <param name="extensionsTypes">Static classes that contain the async operators.</param>
public class AsyncEnumerableTargetMethodExtractor(ImmutableArray<Type> extensionsTypes) : TargetMethodExtractor(extensionsTypes)
{
    /// <inheritdoc/>
    protected override IEnumerable<Type> GetTargetMethodParameterTypes(MethodInfo targetMethod)
    {
        var baseTypes = targetMethod.GetParameters().Select(p => p.ParameterType).ToList();

        var sourceType = baseTypes.First().GetGenericTypeImplementationArgument(typeof(IAsyncEnumerable<>))
                         ?? throw new ArgumentOutOfRangeException(nameof(targetMethod), "Invalid binding method");

        return new[] { typeof(IQueryable<>).MakeGenericType(sourceType) }.Concat(baseTypes.Skip(1));
    }

    /// <summary>Extractor over <c>System.Linq.AsyncEnumerable</c>.</summary>
    public static ITargetMethodExtractor Default { get; } = new AsyncEnumerableTargetMethodExtractor([typeof(AsyncEnumerable)]);
}