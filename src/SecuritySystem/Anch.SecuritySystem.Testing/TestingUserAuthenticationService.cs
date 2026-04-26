using Anch.Core.Auth;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Testing;

public class TestingRawCurrentUser(
    TestRootUserInfo testRootUserInfo,
    RootImpersonateServiceState rootImpersonateServiceState,
    ISyncUserNameResolver userNameResolver) : ICurrentUser
{
    public string Name =>
        rootImpersonateServiceState.CustomUserCredential == null
            ? testRootUserInfo.Name
            : rootImpersonateServiceState.Cache.GetOrAdd(rootImpersonateServiceState.CustomUserCredential, _ =>
                userNameResolver.GetUserName(rootImpersonateServiceState.CustomUserCredential));
}