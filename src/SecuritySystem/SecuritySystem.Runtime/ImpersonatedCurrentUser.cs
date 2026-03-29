using CommonFramework;
using CommonFramework.Auth;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.Credential;
using SecuritySystem.Services;

namespace SecuritySystem;

public class ImpersonatedCurrentUser(
    [FromKeyedServices(ICurrentUser.RawKey)] ICurrentUser rawCurrentUser,
    IImpersonateState impersonateState,
    IUserNameResolver userNameResolver,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource) : ICurrentUser
{
    public string Name
    {
        get
        {
            return impersonateState.CustomUserCredential switch
            {
                null => rawCurrentUser.Name,

                UserCredential.NamedUserCredential namedUserCredential => namedUserCredential.Name,

                _ => defaultCancellationTokenSource.RunSync(ct => userNameResolver.GetUserNameAsync(impersonateState.CustomUserCredential, ct))
            };
        }
    }
}