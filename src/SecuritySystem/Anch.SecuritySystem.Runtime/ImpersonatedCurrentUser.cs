using Anch.Core.Auth;
using Anch.SecuritySystem.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem;

/// <summary>The raw current user, replaced by the impersonated user while code runs under <c>WithImpersonateAsync</c>.</summary>
public class ImpersonatedCurrentUser(
    [FromKeyedServices(ICurrentUser.RawKey)]
    ICurrentUser rawCurrentUser,
    IImpersonateState impersonateState,
    ISyncUserNameResolver userNameResolver) : ICurrentUser
{
    public string Name => impersonateState.CustomUserCredential is null
        ? rawCurrentUser.Name
        : userNameResolver.GetUserName(impersonateState.CustomUserCredential);
}