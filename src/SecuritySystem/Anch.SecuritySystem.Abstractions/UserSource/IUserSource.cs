namespace Anch.SecuritySystem.UserSource;

public interface IUserSource<TUser> : IUserSource
{
    Task<TUser?> TryGetUserAsync(UserCredential userCredential, CancellationToken ct);

    Task<TUser> GetUserAsync(UserCredential userCredential, CancellationToken ct);
}

public interface IUserSource
{
    Type UserType { get; }

    IUserSource<User> ToSimple();
}