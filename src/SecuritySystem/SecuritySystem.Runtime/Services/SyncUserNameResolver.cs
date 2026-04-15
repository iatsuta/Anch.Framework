using CommonFramework;

namespace SecuritySystem.Services;

public class SyncUserNameResolver(
    IUserNameResolver userNameResolver,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null) : ISyncUserNameResolver
{
    public string GetUserName(UserCredential userCredential)
    {
        return userCredential switch
        {
            UserCredential.NamedUserCredential namedUserCredential => namedUserCredential.Name,

            _ => defaultCancellationTokenSource.RunSync(ct => userNameResolver.GetUserNameAsync(userCredential, ct))
        };
    }
}