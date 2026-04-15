using CommonFramework;

namespace SecuritySystem.Services;

public static class ImpersonateServiceExtensions
{
    public static Task WithImpersonateAsync(
        this IImpersonateService impersonateService,
        UserCredential customUserCredential,
        Func<Task> action) =>
        impersonateService.WithImpersonateAsync(
            customUserCredential, action.ToDefaultTask());
}