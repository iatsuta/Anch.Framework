namespace Anch.SecuritySystem.PermissionOptimization;

/// <summary>Reduces the permissions loaded for a check to the smallest equivalent set.</summary>
public interface IRuntimePermissionOptimizationService
{
    IEnumerable<Dictionary<Type, Array>> Optimize(IEnumerable<IReadOnlyDictionary<Type, Array>> permissions);
}
