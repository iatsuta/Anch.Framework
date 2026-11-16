using System.Collections.Immutable;
using System.Reflection;

using Anch.Core;

namespace Anch.GenericQueryable.Services;

/// <summary>
/// Maps <c>GenericXxxAsync(..., ct)</c> to the synchronous operator <c>Xxx(...)</c> (the <c>Async</c> suffix and the cancellation token are dropped).
/// </summary>
/// <param name="extensionsTypes">Static classes that contain the synchronous operators (for example <see cref="System.Linq.Queryable"/>).</param>
public class QueryableTargetMethodExtractor(ImmutableArray<Type> extensionsTypes) : TargetMethodExtractor(extensionsTypes)
{
    /// <inheritdoc/>
    protected override string? TryGetTargetMethodName(MethodInfo baseMethod)
    {
        return base.TryGetTargetMethodName(baseMethod).Maybe(methodName => methodName.EndsWith("Async") ? methodName.SkipLast("Async", true) : null);
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">The last parameter of the method is not a <see cref="CancellationToken"/>.</exception>
    protected override IEnumerable<Type> GetExpectedParameterTypes(MethodInfo baseMethod)
    {
        var parameterTypes = baseMethod.GetParameters().Select(p => p.ParameterType).ToList();

        if (parameterTypes.Last() != typeof(CancellationToken))
        {
            throw new InvalidOperationException(
                $"The last parameter of the method '{baseMethod.Name}' must be of type {nameof(CancellationToken)}.");
        }
        else
        {
            return parameterTypes.SkipLast(1);
        }
    }

    /// <summary>Extractor over <see cref="System.Linq.Queryable"/>.</summary>
    public static ITargetMethodExtractor Default { get; } = new QueryableTargetMethodExtractor([typeof(Queryable)]);
}