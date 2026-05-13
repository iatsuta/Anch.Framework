namespace Anch.SecuritySystem.Services;

public interface IUserNameResolver
{
    Task<string> GetUserNameAsync(UserCredential userCredential, CancellationToken cancellationToken = default);
}

public interface IUserNameResolver<out TUser>
{
    Task<string?> GetUserNameAsync(SecurityRuleCredential credential, CancellationToken cancellationToken = default);
}