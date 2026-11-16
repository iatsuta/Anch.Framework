namespace Anch.SecuritySystem.Notification;

/// <summary>How deep in the hierarchy the restrictions of a permission are.</summary>
public record PermissionLevelInfo<TPermission>
{
    public required TPermission Permission { get; init; }

    public required string LevelInfo { get; init; }
}

/// <summary>The permission level information together with the permission it describes.</summary>
public record FullPermissionLevelInfo<TPermission> : PermissionLevelInfo<TPermission>
{
    public required int Level { get; init; }
}