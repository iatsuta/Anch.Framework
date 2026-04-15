namespace SecuritySystem.UserSource;

public interface IUserSource<TUser> : IUserSource
{
	ValueTask<TUser?> TryGetUserAsync(UserCredential userCredential, CancellationToken cancellationToken = default);

    ValueTask<TUser> GetUserAsync(UserCredential userCredential, CancellationToken cancellationToken = default);
}

public interface IUserSource
{
    Type UserType { get; }

    IUserSource<User> ToSimple();
}