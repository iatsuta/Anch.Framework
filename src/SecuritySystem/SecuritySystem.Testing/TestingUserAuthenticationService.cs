using CommonFramework;
using CommonFramework.Auth;
using SecuritySystem.Services;

namespace SecuritySystem.Testing;

public class TestingRawCurrentUser(
    TestRootUserInfo testRootUserInfo,
    RootImpersonateServiceState rootImpersonateServiceState,
    IUserNameResolver userNameResolver,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null)
    : ICurrentUser
{
    public string Name =>
        rootImpersonateServiceState.CustomUserCredential == null
            ? testRootUserInfo.Name
            : rootImpersonateServiceState.Cache.GetOrAdd(rootImpersonateServiceState.CustomUserCredential, _ =>
                new ImpersonatedCurrentUser(
                    new FixedCurrentUser(testRootUserInfo.Name),
                    rootImpersonateServiceState,
                    userNameResolver,
                    defaultCancellationTokenSource).Name);
}