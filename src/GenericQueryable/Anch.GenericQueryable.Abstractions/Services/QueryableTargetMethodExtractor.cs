using System.Collections.Immutable;
using System.Reflection;
using Anch.Core;

namespace Anch.GenericQueryable.Services;

public class QueryableTargetMethodExtractor(ImmutableArray<Type> extensionsTypes) : TargetMethodExtractor(extensionsTypes)
{
	protected override string? TryGetTargetMethodName(MethodInfo baseMethod)
	{
		return base.TryGetTargetMethodName(baseMethod).Maybe(methodName => methodName.EndsWith("Async") ? methodName.SkipLast("Async", true) : null);
	}

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

	public static ITargetMethodExtractor Default { get; } = new QueryableTargetMethodExtractor([typeof(Queryable)]);
}