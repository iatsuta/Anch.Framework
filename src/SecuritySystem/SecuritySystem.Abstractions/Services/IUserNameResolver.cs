namespace SecuritySystem.Services;

public interface IUserNameResolver
{
    ValueTask<string> GetUserNameAsync(UserCredential userCredential, CancellationToken cancellationToken = default);
}

public interface IUserNameResolver<out TUser>
{
    ValueTask<string?> GetUserNameAsync(SecurityRuleCredential credential, CancellationToken cancellationToken = default);
}