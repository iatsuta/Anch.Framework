using Anch.Core;

namespace Anch.SecuritySystem.Services;

/// <summary>Extensions for <see cref="IImpersonateService"/>.</summary>
public static class ImpersonateServiceExtensions
{
    /// <summary>Runs a code block without a result on behalf of another user.</summary>
    /// <param name="impersonateService">The service.</param>
    /// <param name="customUserCredential">The user to act as.</param>
    /// <param name="action">The code to run.</param>
    public static Task WithImpersonateAsync(
        this IImpersonateService impersonateService,
        UserCredential customUserCredential,
        Func<Task> action) =>
        impersonateService.WithImpersonateAsync(
            customUserCredential, action.ToDefaultTask());
}