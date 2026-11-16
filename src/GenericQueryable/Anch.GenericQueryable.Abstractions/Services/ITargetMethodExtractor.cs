using System.Reflection;

namespace Anch.GenericQueryable.Services;

/// <summary>Finds the real method that implements a <c>GenericXxx</c> operation (for example the EF Core <c>ToListAsync</c>).</summary>
public interface ITargetMethodExtractor
{
    /// <summary>Looks up the implementation of the operation.</summary>
    /// <param name="baseMethod">The <c>GenericXxx</c> method.</param>
    /// <returns>The target method, or <c>null</c> when none is known.</returns>
    MethodInfo? TryGetTargetMethod(MethodInfo baseMethod);
}