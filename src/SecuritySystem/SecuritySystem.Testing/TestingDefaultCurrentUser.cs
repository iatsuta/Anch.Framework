using CommonFramework;
using CommonFramework.Auth;

namespace SecuritySystem.Testing;

public class TestingDefaultCurrentUser(
    TestRootUserInfo testRootUserInfo,
    RootImpersonateServiceState rootImpersonateServiceState,
    ITestingEvaluator<ICurrentUser> currentUserEvaluator,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource) : ICurrentUser
{
    public string Name =>

        rootImpersonateServiceState.CustomUserCredential == null
            ? testRootUserInfo.Name
            : rootImpersonateServiceState.Cache.TryGetValue(rootImpersonateServiceState.CustomUserCredential, out var cachedUserName)
                ? cachedUserName
                : defaultCancellationTokenSource.RunSync(async _ => await currentUserEvaluator.EvaluateAsync(TestingScopeMode.Read, async s => s.Name));
}