using SecuritySystem.Credential;

namespace SecuritySystem.Services;

public interface IUserNameResolver
{
    ValueTask<string> GetUserNameAsync(UserCredential userCredential, CancellationToken cancellationToken = default);
}