namespace Anch.SecuritySystem.UserSource;

public interface IUserSource<TUser> : IUserSource
{
	Task<TUser?> TryGetUserAsync(UserCredential userCredential, CancellationToken cancellationToken = default);

    Task<TUser> GetUserAsync(UserCredential userCredential, CancellationToken cancellationToken = default);
}

public interface IUserSource
{
    Type UserType { get; }

    IUserSource<User> ToSimple();
}