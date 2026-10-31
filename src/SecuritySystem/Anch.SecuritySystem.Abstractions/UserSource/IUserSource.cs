namespace Anch.SecuritySystem.UserSource;

/// <summary>Finds users of the application's own user entity type.</summary>
/// <typeparam name="TUser">The user entity type.</typeparam>
public interface IUserSource<TUser> : IUserSource
{
    /// <summary>Finds a user.</summary>
    /// <param name="userCredential">Name or identifier of the user.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The user, or <c>null</c> if there is none.</returns>
    Task<TUser?> TryGetUserAsync(UserCredential userCredential, CancellationToken ct);

    /// <summary>Finds a user; throws if there is none.</summary>
    /// <param name="userCredential">Name or identifier of the user.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<TUser> GetUserAsync(UserCredential userCredential, CancellationToken ct);
}

/// <summary>Untyped view of a user source.</summary>
public interface IUserSource
{
    /// <summary>The user entity type.</summary>
    Type UserType { get; }

    /// <summary>Returns a view that yields the simple <see cref="User"/> record.</summary>
    IUserSource<User> ToSimple();
}