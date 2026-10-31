namespace Anch.SecuritySystem.UserSource;

/// <summary>Supplies the current user as the application's own user entity.</summary>
/// <typeparam name="TUser">The user entity type.</typeparam>
public interface ICurrentUserSource<out TUser>
{
    /// <summary>The current user.</summary>
    TUser CurrentUser { get; }

    /// <summary>Returns a view that yields the simple <see cref="User"/> record.</summary>
    ICurrentUserSource<User> ToSimple();
}
