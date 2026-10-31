using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

using Anch.Core;

namespace Anch.GenericQueryable.Services;

/// <summary>
/// Base <see cref="ITargetMethodExtractor"/>: maps a method named <c>GenericXxx</c> to a public static method named <c>Xxx</c>
/// on the configured extension types, matching generic arguments and parameter types. Results are cached.
/// </summary>
/// <param name="extensionsTypes">Static classes searched for the target methods (for example the ORM's async extensions).</param>
public class TargetMethodExtractor(ImmutableArray<Type> extensionsTypes) : ITargetMethodExtractor
{
    private readonly ConcurrentDictionary<MethodInfo, MethodInfo?> mappingMethodCache = [];

    /// <inheritdoc/>
    public MethodInfo? TryGetTargetMethod(MethodInfo baseMethod)
    {
        return this.mappingMethodCache.GetOrAdd(baseMethod, _ =>
        {
            if (this.TryGetTargetMethodName(baseMethod) is { } targetMethodName)
            {
                var genericArgs = baseMethod.GetGenericArguments();

                var expectedParameterTypes = this.GetExpectedParameterTypes(baseMethod).ToList();

                var request =

                    from extensionsType in extensionsTypes

                    from method in extensionsType.GetMethods(BindingFlags.Public | BindingFlags.Static)

                    where method.Name == targetMethodName && method.GetGenericArguments().Length == genericArgs.Length

                    let targetMethod = method.IsGenericMethodDefinition ? method.MakeGenericMethod(genericArgs) : method

                    where expectedParameterTypes.SequenceEqual(this.GetTargetMethodParameterTypes(targetMethod))

                    select targetMethod;

                return request.SingleOrDefault();
            }
            else
            {
                return null;
            }
        });
    }

    /// <summary>Maps the name of the <c>GenericXxx</c> method to the target method name; <c>null</c> when the method is not supported.</summary>
    /// <param name="baseMethod">The <c>GenericXxx</c> method.</param>
    protected virtual string? TryGetTargetMethodName(MethodInfo baseMethod)
    {
        return baseMethod.Name.StartsWith("Generic") ? baseMethod.Name.Skip("Generic", true) : null;
    }

    /// <summary>Parameter types the target method is expected to have, derived from the <c>GenericXxx</c> method.</summary>
    /// <param name="baseMethod">The <c>GenericXxx</c> method.</param>
    protected virtual IEnumerable<Type> GetExpectedParameterTypes(MethodInfo baseMethod)
    {
        return baseMethod.GetParameters().Select(p => p.ParameterType);
    }

    /// <summary>Parameter types of a candidate target method, normalized for comparison with <see cref="GetExpectedParameterTypes"/>.</summary>
    /// <param name="targetMethod">The candidate method.</param>
    protected virtual IEnumerable<Type> GetTargetMethodParameterTypes(MethodInfo targetMethod)
    {
        return targetMethod.GetParameters().Select(p => p.ParameterType);
    }
}