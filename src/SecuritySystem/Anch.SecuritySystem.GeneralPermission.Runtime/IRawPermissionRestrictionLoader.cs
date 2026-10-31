namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Loads the restrictions of a permission in the raw form.</summary>
public interface IRawPermissionRestrictionLoader<in TPermission>
{
    Task<Dictionary<Type, Array>> LoadAsync(TPermission permission, CancellationToken ct);
}