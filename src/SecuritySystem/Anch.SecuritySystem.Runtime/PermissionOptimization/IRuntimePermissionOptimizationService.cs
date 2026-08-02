namespace Anch.SecuritySystem.PermissionOptimization;

public interface IRuntimePermissionOptimizationService
{
    IEnumerable<Dictionary<Type, Array>> Optimize(IEnumerable<IReadOnlyDictionary<Type, Array>> permissions);
}
