using Anch.Core.Auth;
using Anch.SecuritySystem.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem;

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