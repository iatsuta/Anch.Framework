using CommonFramework.Auth;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.Services;

namespace SecuritySystem;

public class ImpersonatedCurrentUser(
    [FromKeyedServices(ICurrentUser.RawKey)]
    ICurrentUser rawCurrentUser,
    IImpersonateState impersonateState,
    ISyncUserNameResolver userNameResolver) : ICurrentUser
{
    public string Name => impersonateState.CustomUserCredential == null
        ? rawCurrentUser.Name
        : userNameResolver.GetUserName(impersonateState.CustomUserCredential);
}