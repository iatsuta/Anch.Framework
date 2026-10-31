using System.Reflection;

using Anch.Core;

namespace Anch.GenericQueryable.Services;

/// <summary>Supports only <c>GenericAsAsyncEnumerable</c>, mapping it to <c>AsyncEnumerable.ToAsyncEnumerable</c>.</summary>
public class CastToAsyncEnumerableTargetMethodExtractor() : TargetMethodExtractor([typeof(AsyncEnumerable)])
{
    private readonly Dictionary<string, MethodInfo> allowedMethods = new()
    {
        {
            nameof(GenericQueryableExtensions.GenericAsAsyncEnumerable),
            typeof(AsyncEnumerable).GetMethod(nameof(AsyncEnumerable.ToAsyncEnumerable))!
        }
    };

    /// <inheritdoc/>
    protected override IEnumerable<Type> GetTargetMethodParameterTypes(MethodInfo targetMethod)
    {
        var baseTypes = targetMethod.GetParameters().Select(p => p.ParameterType).ToList();

        var sourceType = baseTypes.First().GetGenericTypeImplementationArgument(typeof(IEnumerable<>))
                         ?? throw new ArgumentOutOfRangeException(nameof(targetMethod), "Invalid binding method");

        return new[] { typeof(IQueryable<>).MakeGenericType(sourceType) }.Concat(baseTypes.Skip(1));
    }


    /// <inheritdoc/>
    protected override string? TryGetTargetMethodName(MethodInfo baseMethod) => this.allowedMethods.GetValueOrDefault(baseMethod.Name)?.Name;
}