using Anch.Core.Auth;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Testing;

/// <summary>The raw current user in tests: the test root user unless a user is impersonated.</summary>
public class TestingRawCurrentUser(
    TestRootUserInfo testRootUserInfo,
    RootImpersonateServiceState rootImpersonateServiceState,
    ISyncUserNameResolver userNameResolver) : ICurrentUser
{
    public string Name =>
        rootImpersonateServiceState.CustomUserCredential is null
            ? testRootUserInfo.Name
            : rootImpersonateServiceState.Cache.GetOrAdd(rootImpersonateServiceState.CustomUserCredential, _ =>
                userNameResolver.GetUserName(rootImpersonateServiceState.CustomUserCredential));
}