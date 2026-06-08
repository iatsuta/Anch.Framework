namespace Anch.SecuritySystem.Services;

public interface IUserNameResolver
{
    Task<string> GetUserNameAsync(UserCredential userCredential, CancellationToken ct);
}

public interface IUserNameResolver<out TUser>
{
    Task<string?> GetUserNameAsync(SecurityRuleCredential credential, CancellationToken ct);
}