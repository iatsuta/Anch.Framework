using Anch.Core;

namespace Anch.SecuritySystem.Services;

public class SyncUserNameResolver(
    IUserNameResolver userNameResolver,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null) : ISyncUserNameResolver
{
    public string GetUserName(UserCredential userCredential)
    {
        return userCredential switch
        {
            UserCredential.NamedUserCredential namedUserCredential => namedUserCredential.Name,

            UserCredential.FullUserCredential fullUserCredential => fullUserCredential.User.Name,

            _ => defaultCancellationTokenSource.RunSync(ct => userNameResolver.GetUserNameAsync(userCredential, ct))
        };
    }
}